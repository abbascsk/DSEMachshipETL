using System.Xml.Serialization;
using AutoMapper;
using DSEConETL.Data;
using DSEConETL.Entities;
using DSEConETL.Extensions;
using DSEConETL.Models;
using DSEConETL.Models.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DSEConETL.Services;

public class DseXmlEtlService(IOptions<GeneralSettings> generalSettings, IOptions<EmailSettings> emailSettings, 
    IMapper mapper, DseDbContext dbContext, Logger logger, EmailService emailService)
{
    public async Task GetNewConsignments()
    {
        try
        {
            var srcDir = generalSettings.Value.SourceDirectory;
            var destDir = generalSettings.Value.DestinationDirectory;

            Directory.CreateDirectory(destDir);
            
            var files = Directory.GetFiles(srcDir, "*.xml");

            Console.WriteLine($"DSE XML Parser: {files.Length} files found");

            foreach (var file in files)
            {
                try
                {
                    var destFile = Path.Combine(destDir, Path.GetFileName(file));
                    Console.WriteLine($"DSE XML Parser: Reading file {file}");

                    // Remove empty tags from the file (Filtering null data)
                    string fileContent = await File.ReadAllTextAsync(file);
                    fileContent = fileContent.XmlRemoveEmptyTags();

                    var serializer = new XmlSerializer(typeof(ConsignmentXmlRequest));
                    using var reader = new StringReader(fileContent);
                    var request = (ConsignmentXmlRequest?)serializer.Deserialize(reader);

                    if (request == null)
                        throw new Exception($"Unable to parse the file \"{file}\"");

                    var customer = await dbContext.Customers
                        .Where(x => x.customer_code.ToUpper() == request.AccountCode.ToUpper().Trim())
                        .Include(customer => customer.Branch)
                        .FirstOrDefaultAsync();
                    
                    if(customer == null)
                        throw new Exception($"Customer account code is not found \"{request.AccountCode}\"");

                    var notificationEmail =
                        customer.Branch.api_notification_email ?? emailSettings.Value.DefaultToEmail;

                    var customerSetting =
                        await dbContext.CustomerSettings.FirstOrDefaultAsync(x =>
                            x.customer_id == customer.customer_id);
                    
                    if(request.Items == null)
                        throw new Exception($"No items in consignment");
                    
                    var conServiceType =
                        await dbContext.ConsignmentServiceTypes.FirstOrDefaultAsync(x =>
                            x.consignment_service_type_name == request.ServiceType);
                    
                    if(conServiceType == null)
                        throw new Exception($"Service type is invalid \"{request.ServiceType}\"");

                    PalletType? palletType = null;
                    PalletExchangeType? palletExchangeType = null;
                    
                    if (!String.IsNullOrWhiteSpace(request.PalletType))
                    {
                        palletType =
                            await dbContext.PalletTypes.FirstOrDefaultAsync(x =>
                                x.pallet_type_name == request.PalletType);
                    
                        if(palletType == null)
                            throw new Exception($"Pallet type is invalid \"{request.PalletType}\"");
                    }
                    
                    if (!String.IsNullOrWhiteSpace(request.PalletExchangeType))
                    {
                        palletExchangeType =
                            await dbContext.PalletExchangeTypes.FirstOrDefaultAsync(x =>
                                x.pallet_exchange_type_name == request.PalletExchangeType);
                    
                        if(palletExchangeType == null)
                            throw new Exception($"Pallet type is invalid \"{request.PalletExchangeType}\"");
                    }

                    var consignment = mapper.Map<Consignment>(request);

                    consignment.customer_id = customer.customer_id;
                    consignment.consignment_status_type_id = generalSettings.Value.DefaultConStatusTypeId;
                    consignment.consignment_service_type_id = conServiceType.consignment_service_type_id;
                    consignment.pallet_type_id = palletType?.pallet_type_id;
                    consignment.pallet_exchange_type_id = palletExchangeType?.pallet_exchange_type_id;
                    consignment.billing_template_id = customerSetting?.billing_template_id ?? generalSettings.Value.DefaultBillingTemplateId;
                    consignment.created_at = DateTime.Now;
                    consignment.created_by = generalSettings.Value.CreatedByName;
                    
                    foreach (var item in request.Items)
                    {
                        var unit = await dbContext.Units.FirstOrDefaultAsync(x => x.unit_name == item.UnitName);
                    
                        if(unit == null)
                            throw new Exception($"Unit type is invalid \"{item.UnitName}\"");

                        item.UnitId = unit.unit_id;
                    }

                    var consignmentItems = request.Items
                        .Select(mapper.Map<ConsignmentItem>)
                        .ToList();

                    var result = await dbContext.Consignments.AddAsync(consignment);
                    await dbContext.SaveChangesAsync();

                    // Update consignment no
                    await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.[sp_update_consignment_no] @consignment_id = {consignment.consignment_id}");
                    
                    var lineNo = 1;
                    consignmentItems.ForEach(item =>
                    {
                        item.consignment_id = consignment.consignment_id;
                        item.line_no = lineNo;
                        item.created_at = DateTime.Now;
                        item.created_by = generalSettings.Value.CreatedByName;

                        lineNo++;
                    });
                    
                    await dbContext.ConsignmentItems.AddRangeAsync(consignmentItems);
                    await dbContext.SaveChangesAsync();

                    var con_id = consignment.consignment_id;
                    consignment = await
                        dbContext.Consignments.FirstOrDefaultAsync(x => x.consignment_id == con_id);
                    
                    Console.WriteLine($"DSE XML Parser: Consignment created {consignment.consignment_id}");
                    emailService.SendEmailWithAttachment("system@dsetrucks.com.au", "DSE Con ETL", notificationEmail, "New Job received", $"<p>New consignment created: ${consignment.consignment_no_full}</p>", file);
                    
                    File.Move(file, destFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"DSE XML Parser Error: {ex.InnerException?.Message ?? ex.Message}");
                    logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DSE XML Parser Error: {ex.InnerException?.Message ?? ex.Message}");
            logger.LogError(ex.InnerException?.Message ?? ex.Message);
            // throw;
        }
    }
}