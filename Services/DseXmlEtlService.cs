using System.Xml.Serialization;
using AutoMapper;
using DSEConETL.Data;
using DSEConETL.Entities;
using DSEConETL.Models;
using DSEConETL.Models.Xml;
using Microsoft.Extensions.Options;

namespace DSEConETL.Services;

public class DseXmlEtlService(IOptions<GeneralSettings> generalSettings, IMapper mapper, DseDbContext dbContext)
{
    public async Task GetNewConsignments()
    {
        try
        {
            var srcDir = generalSettings.Value.SourceDirectory;
            var files = Directory.GetFiles(srcDir, "*.xml");

            Console.WriteLine($"DSE XML Parser: {files.Length} files found");

            foreach (var file in files)
            {
                Console.WriteLine($"DSE XML Parser: Reading file {file}");

                var serializer = new XmlSerializer(typeof(ConsignmentXmlRequest));
                await using var fileStream = new FileStream(file, FileMode.Open);
                var request = (ConsignmentXmlRequest?)serializer.Deserialize(fileStream);

                if (request == null)
                    throw new Exception($"DSE XML Parser: Unable to parse the file \"{file}\"");

                var consignment = mapper.Map<Consignment>(request);
                consignment.consignment_status_type_id = generalSettings.Value.DefaultConStatusTypeId;
                consignment.created_at = DateTime.Now;
                consignment.created_by = "DSEConETL";

                var result = await dbContext.Consignments.AddAsync(consignment);
                await dbContext.SaveChangesAsync();

                Console.WriteLine($"DSE XML Parser: Consignment created {consignment.consignment_id}");

                var consignmentItems = request.Items?.Select(mapper.Map<ConsignmentItem>).ToList();

                if (consignmentItems == null)
                    continue;

                consignmentItems.ForEach(item =>
                {
                    item.consignment_id = consignment.consignment_id;
                });
                
                await dbContext.ConsignmentItems.AddRangeAsync(consignmentItems);
                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DSE XML Parser Error: {ex.InnerException?.Message ?? ex.Message}");
            // throw;
        }
    }
}