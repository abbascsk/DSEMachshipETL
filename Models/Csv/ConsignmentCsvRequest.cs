using System;
using CsvHelper.Configuration.Attributes;


public class ConsignmentCsvRequest
{
    public string Connote { get; set; }
    
    [Format("dd/MM/yyyy")]
    public DateTime? Released { get; set; }
    
    public string Reference { get; set; }
    public string Service { get; set; }
    public string Sender { get; set; }
    public string SAddress1 { get; set; }
    public string SAddress2 { get; set; }
    public string SLocality { get; set; }
    public string SState { get; set; }
    public string SPostcode { get; set; }
    public string Receiver { get; set; }
    public string RAddress1 { get; set; }
    public string RAddress2 { get; set; }
    public string RLocality { get; set; }
    public string RState { get; set; }
    public string RPostcode { get; set; }
    public int? Items { get; set; } // Optional
    public decimal Weight { get; set; }
    public decimal Volume { get; set; }
    public string Instructions { get; set; }
    public string DgClass { get; set; }
    public string UNNumber { get; set; }
    public string PackingGroup { get; set; }
    public string AggregateQuantity { get; set; }
    public string AccountCode { get; set; }
    public DateTime? PickupDateTime { get; set; }
    public DateTime? PickupClosingDateTime { get; set; }
    public string SContactName { get; set; }
    public string SContactNumber { get; set; }
    public string SEmail { get; set; }
    public string RContactName { get; set; }
    public string RContactNumber { get; set; }
    public string REmail { get; set; }

    public void TrimAllStringFields()
    {
        var stringProperties = this.GetType().GetProperties()
            .Where(p => p.PropertyType == typeof(string) && p.CanRead && p.CanWrite);

        foreach (var prop in stringProperties)
        {
            var value = (string)prop.GetValue(this);
            if (value != null)
            {
                prop.SetValue(this, value.Trim());
            }
        }
    }
}
