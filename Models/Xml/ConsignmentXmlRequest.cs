using System.Xml.Serialization;

namespace DSEConETL.Models.Xml;

[XmlRoot("Consignment")]
public class ConsignmentXmlRequest
{
    public string ServiceType { get; set; }
    public string? ConsignmentInstructions { get; set; }
    public DateTime? DispatchDate { get; set; }
    public string? SenderName { get; set; }
    public string? SenderStreet { get; set; }
    public string? SenderSuburb { get; set; }
    public string? SenderState { get; set; }
    public string? SenderPostcode { get; set; }
    public string? SenderCountry { get; set; }
    public string? SenderEmail { get; set; }
    public string? SenderContact { get; set; }
    public string? SenderPhone { get; set; }
    public string? SenderMobile { get; set; }
    public string? SenderSiteInfo { get; set; }
    public string? SenderRef { get; set; }
    public string? PickupInstructions { get; set; }
    public DateTime? PickupStartTime { get; set; }
    public DateTime? PickupEndTime { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverStreet { get; set; }
    public string? ReceiverSuburb { get; set; }
    public string? ReceiverState { get; set; }
    public string? ReceiverPostcode { get; set; }
    public string? ReceiverCountry { get; set; }
    public string? ReceiverEmail { get; set; }
    public string? ReceiverContact { get; set; }
    public string? ReceiverPhone { get; set; }
    public string? ReceiverMobile { get; set; }
    public string? ReceiverSiteInfo { get; set; }
    public string? ReceiverRef { get; set; }
    public string? DeliveryInstructions { get; set; }
    public DateTime? DeliveryStartTime { get; set; }
    public DateTime? DeliveryEndTime { get; set; }
    public bool? DangerousGoods { get; set; }
    public bool? TailLift { get; set; }
    public bool? Atl { get; set; }
    public bool? HandUnload { get; set; }
    public bool? TwoMenRequired { get; set; }
    public string? PalletType { get; set; }
    public string? PalletExchangeType { get; set; }
    
    [XmlArray("Items")]
    [XmlArrayItem("Item")]
    public List<ConsignmentItemXmlRequest>? Items { get; set; }
}

public class ConsignmentItemXmlRequest
{
    public string UnitName { get; set; }
    public int? LineNo { get; set; }
    public string ItemDescription { get; set; }
    public decimal? Weight { get; set; }
    public int? Length { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public bool? Stackable { get; set; }
    public bool? Reorient { get; set; }
    public string? Sku { get; set; }
    public string? ItemReference { get; set; }
    public string? Barcode { get; set; }
}