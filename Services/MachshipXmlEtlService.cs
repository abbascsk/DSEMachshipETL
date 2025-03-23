using System.Text;
using System.Xml.Serialization;
using AutoMapper;
using DSEMachshipETL.Data;
using DSEMachshipETL.Entities;
using DSEMachshipETL.Extensions;
using DSEMachshipETL.Models;
using DSEMachshipETL.Models.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DSEMachshipETL.Services;

public class MachshipXmlEtlService(IOptions<GeneralSettings> generalSettings, IOptions<EmailSettings> emailSettings, 
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

            Console.WriteLine($"Machship XML Parser: {files.Length} files found");

            foreach (var file in files)
            {
                try
                {
                    var destFile = Path.Combine(destDir, Path.GetFileName(file));
                    Console.WriteLine($"Machship XML Parser: Reading file {file}");

                    // Remove empty tags from the file (Filtering null data)
                    string fileContent = await File.ReadAllTextAsync(file);
                    fileContent = fileContent.XmlRemoveEmptyTags();

                    var serializer = new XmlSerializer(typeof(ManifestXmlRequest));
                    using var reader = new StringReader(fileContent);
                    var request = (ManifestXmlRequest?)serializer.Deserialize(reader);

                    if (request == null)
                        throw new Exception($"Unable to parse the file: \"{file}\"");

                    var customer = await dbContext.Customers
                        .Where(x => x.customer_code.ToUpper() == request.AccountNumber.ToUpper().Trim())
                        .Include(customer => customer.Branch)
                        .FirstOrDefaultAsync();
                    
                    if(customer == null)
                        throw new Exception($"Customer account code is not found: \"{request.AccountNumber}\" in file: \"{file}\"");

                    var notificationEmail =
                        customer.Branch.api_notification_email ?? emailSettings.Value.DefaultToEmail;

                    var customerSetting =
                        await dbContext.CustomerSettings.FirstOrDefaultAsync(x =>
                            x.customer_id == customer.customer_id);

                    StringBuilder sb = new StringBuilder();
                    
                    foreach (var consignmentRequest in request.Consignments.ConsignmentList)
                    {
                        try
                        {
                            consignmentRequest.TrimAllStringFields();
                            
                            if(consignmentRequest.Items?.ItemList == null || consignmentRequest.Items?.ItemList.Count == 0)
                                throw new Exception($"file: \"{file}\" No items in consignment: \"{consignmentRequest.ConnoteNumber}\"");
                            
                            // var conServiceType =
                            //     await dbContext.ConsignmentServiceTypes.FirstOrDefaultAsync(x =>
                            //         x.consignment_service_type_name == request.ServiceType);
                            //
                            // if(conServiceType == null)
                            //     throw new Exception($"Service type is invalid \"{request.ServiceType}\"");

                            // PalletType? palletType = null;
                            // PalletExchangeType? palletExchangeType = null;
                            //
                            // if (!String.IsNullOrWhiteSpace(request.PalletType))
                            // {
                            //     palletType =
                            //         await dbContext.PalletTypes.FirstOrDefaultAsync(x =>
                            //             x.pallet_type_name == request.PalletType);
                            //
                            //     if(palletType == null)
                            //         throw new Exception($"Pallet type is invalid \"{request.PalletType}\"");
                            // }
                            //
                            // if (!String.IsNullOrWhiteSpace(request.PalletExchangeType))
                            // {
                            //     palletExchangeType =
                            //         await dbContext.PalletExchangeTypes.FirstOrDefaultAsync(x =>
                            //             x.pallet_exchange_type_name == request.PalletExchangeType);
                            //
                            //     if(palletExchangeType == null)
                            //         throw new Exception($"Pallet type is invalid \"{request.PalletExchangeType}\"");
                            // }

                            // var consignment = mapper.Map<Consignment>(request);
                            
                            var conServiceType = dbContext.ConsignmentServiceTypes
                                .OrderBy(x => x.sort_order)
                                .FirstOrDefault();

                            var consignment = new Consignment
                            {
                                customer_id = customer.customer_id,
                                consignment_status_type_id = generalSettings.Value.DefaultConStatusTypeId,
                                consignment_service_type_id = conServiceType?.consignment_service_type_id,
                                // consignment.pallet_type_id = palletType?.pallet_type_id;
                                // consignment.pallet_exchange_type_id = palletExchangeType?.pallet_exchange_type_id;
                                edi_id = consignmentRequest.ConnoteNumber ?? "",
                                billing_template_id = customerSetting?.billing_template_id ?? generalSettings.Value.DefaultBillingTemplateId,
                                sender_name = consignmentRequest.FromAddress?.Name ?? "",
                                sender_street = $"{consignmentRequest.FromAddress?.AddressLine1 ?? ""} {consignmentRequest.FromAddress?.AddressLine2 ?? ""}",
                                sender_suburb = consignmentRequest.FromAddress?.Location.Suburb ?? "",
                                sender_state = consignmentRequest.FromAddress?.Location.State ?? "",
                                sender_postcode = consignmentRequest.FromAddress?.Location.Postcode ?? "",
                                sender_contact = consignmentRequest.FromAddress?.Contact ?? "",
                                sender_phone = consignmentRequest.FromAddress?.Phone ?? "",
                                pickup_start_time = consignmentRequest.PickupTime ?? consignmentRequest.DespatchDate ?? DateTime.Today.AddDays(1),
                                pickup_end_time = consignmentRequest.ClosingTime,
                                receiver_name = consignmentRequest.ToAddress?.Name ?? "",
                                receiver_street = $"{consignmentRequest.ToAddress?.AddressLine1 ?? ""} {consignmentRequest.ToAddress?.AddressLine2 ?? ""}",
                                receiver_suburb = consignmentRequest.ToAddress?.Location.Suburb ?? "",
                                receiver_state = consignmentRequest.ToAddress?.Location.State ?? "",
                                receiver_postcode = consignmentRequest.ToAddress?.Location.Postcode ?? "",
                                receiver_contact = consignmentRequest.ToAddress?.Contact ?? "",
                                receiver_phone = consignmentRequest.ToAddress?.Phone ?? "",
                                delivery_instructions = consignmentRequest.DeliverySpecialInstructions ?? "",
                                created_at = DateTime.Now,
                                created_by = generalSettings.Value.CreatedByName,
                            };

                            foreach (var item in consignmentRequest.Items?.ItemList)
                            {
                                var unit = await dbContext.Units.FirstOrDefaultAsync(x => x.unit_name == item.CarrierItemTypeName);
                            
                                if(unit == null)
                                    throw new Exception($"file: \"{file}\" Unit type is invalid \"{item.CarrierItemTypeName}\"");

                                item.UnitId = unit.unit_id;
                            }

                            // Create consignment in Database
                            await dbContext.Consignments.AddAsync(consignment);
                            await dbContext.SaveChangesAsync();

                            // Update consignment no
                            await dbContext.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.[sp_update_consignment_no] @consignment_id = {consignment.consignment_id}");

                            var consignmentItems = new List<ConsignmentItem>();

                            var lineNo = 1;
                            foreach (var item in consignmentRequest.Items?.ItemList)
                            {
                                var consignmentItem = new ConsignmentItem();
                                
                                consignmentItem.consignment_id = consignment.consignment_id;
                                consignmentItem.unit_id = item.UnitId;
                                consignmentItem.line_no = lineNo;
                                consignmentItem.item_description = item.Description;
                                consignmentItem.length = Convert.ToInt32(item.Length);
                                consignmentItem.width = Convert.ToInt32(item.Width);
                                consignmentItem.height = Convert.ToInt32(item.Height);
                                consignmentItem.weight = item.Weight;
                                consignmentItem.barcode = item.Barcode;
                                consignmentItem.created_at = DateTime.Now;
                                consignmentItem.created_by = generalSettings.Value.CreatedByName;
                                
                                lineNo++;

                                consignmentItems.Add(consignmentItem);
                            }
                            
                            await dbContext.ConsignmentItems.AddRangeAsync(consignmentItems);
                            await dbContext.SaveChangesAsync();
                            
                            var con_id = consignment.consignment_id;
                            consignment = await
                                dbContext.Consignments.FirstOrDefaultAsync(x => x.consignment_id == con_id);
                            
                            Console.WriteLine($"Machship XML Parser: Consignment created {consignment.consignment_id}");
                            sb.AppendLine($"<p>New consignment created: ${consignment.consignment_no_full}</p>");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Machship XML Parser Error: {ex.InnerException?.Message ?? ex.Message}");
                            logger.LogError(ex.InnerException?.Message ?? ex.Message);
                        }
                    }

                    emailService.SendEmailWithAttachment("system@dsetrucks.com.au", "DSE Machship Con ETL", notificationEmail, "New consignment(s) received", sb.ToString(), file);
                    
                    File.Move(file, destFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Machship XML Parser Error: {ex.InnerException?.Message ?? ex.Message}");
                    logger.LogError(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Machship XML Parser Error: {ex.InnerException?.Message ?? ex.Message}");
            logger.LogError(ex.InnerException?.Message ?? ex.Message);
            // throw;
        }
    }
}