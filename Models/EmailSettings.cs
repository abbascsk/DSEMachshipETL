namespace DSEMachshipETL.Models;

public class EmailSettings
{
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public string ErrorEmail { get; set; }
    public string DefaultToEmail{ get; set; }
}