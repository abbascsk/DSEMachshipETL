using System.Globalization;
using System.Text;
using System.Xml.Serialization;
using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using DSEMachshipETL.Data;
using DSEMachshipETL.Entities;
using DSEMachshipETL.Extensions;
using DSEMachshipETL.Models;
using DSEMachshipETL.Models.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DSEMachshipETL.Services;

public class MachshipCsvEtlService(
    IOptions<GeneralSettings> generalSettings,
    IOptions<EmailSettings> emailSettings,
    DseDbContext dbContext,
    Logger logger,
    EmailService emailService)
{
    public async Task GetNewConsignments()
    {
        try
        {
            var srcDir = generalSettings.Value.SourceDirectory;
            var destDir = generalSettings.Value.DestinationDirectory;

            Directory.CreateDirectory(destDir);

            var files = Directory.GetFiles(srcDir, "*.csv");

            Console.WriteLine($"Machship CSV Parser: {files.Length} files found");

            foreach (var file in files)
            {
                try
                {
                    var destFile = Path.Combine(destDir, Path.GetFileName(file));
                    Console.WriteLine($"Machship CSV Parser: Reading file {file}");

                    // Remove empty tags from the file (Filtering null data)
                    string fileContent = await File.ReadAllTextAsync(file);
                    using var reader = new StringReader(fileContent);

                    using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                        {
                            HasHeaderRecord = true,
                            MissingFieldFound = null // Ignore missing fields
                        }
                    );

                    var records = csv.GetRecords<ConsignmentCsvRequest>();

                    if (records == null)
                        throw new Exception($"Unable to parse the file: \"{file}\"");

                    foreach (var consignmentRequest in records)
                    {
                        try
                        {
                            consignmentRequest.TrimAllStringFields();

                            var customer = await dbContext.Customers
                                .Where(x => x.customer_code.ToUpper() ==
                                            consignmentRequest.AccountCode.ToUpper().Trim())
                                .Include(customer => customer.Branch)
                                .FirstOrDefaultAsync();

                            if (customer == null)
                                throw new Exception(
                                    $"Customer account code is not found: \"{consignmentRequest.AccountCode}\" in file: \"{file}\"");

                            var notificationEmail =
                                customer.Branch.api_notification_email ?? emailSettings.Value.DefaultToEmail;

                            var customerSetting =
                                await dbContext.CustomerSettings.FirstOrDefaultAsync(x =>
                                    x.customer_id == customer.customer_id);

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
                                edi_id = consignmentRequest.Connote,
                                billing_template_id = customerSetting?.billing_template_id ??
                                                      generalSettings.Value.DefaultBillingTemplateId,
                                sender_name = consignmentRequest.Sender,
                                sender_street =
                                    $"{consignmentRequest.SAddress1} {consignmentRequest.SAddress2}",
                                sender_suburb = consignmentRequest.SLocality,
                                sender_state = consignmentRequest.SState,
                                sender_postcode = consignmentRequest.SPostcode,
                                sender_contact = consignmentRequest.SContactName,
                                sender_phone = consignmentRequest.SContactNumber,
                                sender_email = consignmentRequest.SEmail,
                                pickup_start_time = consignmentRequest.PickupDateTime,
                                pickup_end_time = consignmentRequest.PickupClosingDateTime,
                                receiver_name = consignmentRequest.Receiver,
                                receiver_street =
                                    $"{consignmentRequest.RAddress1} {consignmentRequest.RAddress2}",
                                receiver_suburb = consignmentRequest.RLocality,
                                receiver_state = consignmentRequest.RState,
                                receiver_postcode = consignmentRequest.RPostcode,
                                receiver_contact = consignmentRequest.RContactName,
                                receiver_phone = consignmentRequest.RContactNumber,
                                receiver_email = consignmentRequest.REmail,
                                consignment_instructions = consignmentRequest.Instructions,
                                created_at = DateTime.Now,
                                created_by = generalSettings.Value.CreatedByName,
                            };

                            // Create consignment in Database
                            await dbContext.Consignments.AddAsync(consignment);
                            await dbContext.SaveChangesAsync();

                            // Update consignment no
                            await dbContext.Database.ExecuteSqlInterpolatedAsync(
                                $"EXEC dbo.[sp_update_consignment_no] @consignment_id = {consignment.consignment_id}");

                            var consignmentItem = new ConsignmentItem
                            {
                                consignment_id = consignment.consignment_id,
                                line_no = 1,
                                item_description = $"Items: {consignmentRequest.Items} Volume: {consignmentRequest.Volume} AggQty: {consignmentRequest.AggregateQuantity} DGClass: {consignmentRequest.DgClass} UNN: {consignmentRequest.UNNumber} PkgGrp: {consignmentRequest.PackingGroup}",
                                weight = consignmentRequest.Weight,
                                created_at = DateTime.Now,
                                created_by = generalSettings.Value.CreatedByName
                            };

                            await dbContext.ConsignmentItems.AddAsync(consignmentItem);
                            await dbContext.SaveChangesAsync();

                            var con_id = consignment.consignment_id;
                            consignment = await
                                dbContext.Consignments.FirstOrDefaultAsync(x => x.consignment_id == con_id);

                            Console.WriteLine($"Machship CSV Parser: Consignment created {consignment.consignment_id}");

                            emailService.SendEmailWithAttachment("system@dsetrucks.com.au", "DSE Machship Con ETL",
                                notificationEmail, "New consignment(s) received",
                                $"<p>New consignment created: ${consignment.consignment_no_full}</p>", file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Machship CSV Parser Error: {ex.InnerException?.Message ?? ex.Message}");
                            logger.LogError($"File: \"{file}\" {ex.InnerException?.Message ?? ex.Message}");
                        }
                    }

                    File.Move(file, destFile);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Machship CSV Parser Error: {ex.InnerException?.Message ?? ex.Message}");
                    logger.LogError($"File: \"{file}\" {ex.InnerException?.Message ?? ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Machship CSV Parser Error: {ex.InnerException?.Message ?? ex.Message}");
            logger.LogError(ex.InnerException?.Message ?? ex.Message);
            // throw;
        }
    }
}