namespace DSEMachshipETL.Models;

public class GeneralSettings
{
    public string SourceDirectory { get; set; }
    public string DestinationDirectory { get; set; }
    public int DefaultConStatusTypeId { get; set; }
    public int DefaultBillingTemplateId { get; set; }
    public string CreatedByName { get; set; }
}