using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace DSEMachshipETL.Models.Xml;

[XmlRoot("Manifest")]
[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Skip)]
public class ManifestXmlRequest
{
    public string AccountNumber { get; set; }
    public string PayingAccount { get; set; }
    public ConsignmentsXmlRequest Consignments { get; set; }

    public void TrimAllStringFields()
    {
        AccountNumber = AccountNumber?.Trim();
        PayingAccount = PayingAccount?.Trim();
        Consignments?.TrimAllStringFields();
    }
}

public class ConsignmentsXmlRequest
{
    [XmlElement("Consignment")]
    public List<ConsignmentXmlRequest> ConsignmentList { get; set; }

    public void TrimAllStringFields()
    {
        if (ConsignmentList != null)
        {
            foreach (var consignment in ConsignmentList)
            {
                consignment.TrimAllStringFields();
            }
        }
    }
}

public class ConsignmentXmlRequest
{
    public string? ConnoteNumber { get; set; }
    public string? PickupRequired { get; set; }

    [XmlElement(ElementName = "PickupTime", IsNullable = true)]
    public string? PickupTimeString { get; set; }

    [XmlElement(ElementName = "ClosingTime", IsNullable = true)]
    public string? ClosingTimeString { get; set; }

    [XmlElement(ElementName = "DespatchDate", IsNullable = true)]
    public DateTime? DespatchDate { get; set; }
    public string? ServiceCode { get; set; }
    public string? Reference1 { get; set; }
    public string? Reference2 { get; set; }
    public string? DeliverySpecialInstructions { get; set; }
    public bool ContainsDangerousGoods { get; set; }
    public AddressXmlRequest? FromAddress { get; set; }
    public AddressXmlRequest? ToAddress { get; set; }
    public ItemsXmlRequest? Items { get; set; }

    [XmlIgnore]
    public DateTime? PickupTime
    {
        get => string.IsNullOrEmpty(PickupTimeString) ? (DateTime?)null : DateTime.Parse(PickupTimeString);
        set => PickupTimeString = value?.ToString("o");
    }

    [XmlIgnore]
    public DateTime? ClosingTime
    {
        get => string.IsNullOrEmpty(ClosingTimeString) ? (DateTime?)null : DateTime.Parse(ClosingTimeString);
        set => ClosingTimeString = value?.ToString("o");
    }

    public void TrimAllStringFields()
    {
        ConnoteNumber = ConnoteNumber?.Trim();
        PickupRequired = PickupRequired?.Trim();
        ServiceCode = ServiceCode?.Trim();
        Reference1 = Reference1?.Trim();
        Reference2 = Reference2?.Trim();
        DeliverySpecialInstructions = DeliverySpecialInstructions?.Trim();
        FromAddress?.TrimAllStringFields();
        ToAddress?.TrimAllStringFields();
        Items?.TrimAllStringFields();
    }
}

public class AddressXmlRequest
{
    public string Name { get; set; }
    public string Contact { get; set; }
    public string Phone { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public LocationXmlRequest Location { get; set; }

    public void TrimAllStringFields()
    {
        Name = Name?.Trim();
        Phone = Phone?.Trim();
        AddressLine1 = AddressLine1?.Trim();
        AddressLine2 = AddressLine2?.Trim();
        Location?.TrimAllStringFields();
    }
}

public class LocationXmlRequest
{
    public string Suburb { get; set; }
    public string State { get; set; }
    public string Postcode { get; set; }

    public void TrimAllStringFields()
    {
        Suburb = Suburb?.Trim();
        State = State?.Trim();
        Postcode = Postcode?.Trim();
    }
}

public class ItemsXmlRequest
{
    [XmlElement("Item")]
    public List<ItemXmlRequest> ItemList { get; set; }

    public void TrimAllStringFields()
    {
        if (ItemList != null)
        {
            foreach (var item in ItemList)
            {
                item.TrimAllStringFields();
            }
        }
    }
}

public class ItemXmlRequest
{
    public string Description { get; set; }
    public string ItemType { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Weight { get; set; }
    public string Barcode { get; set; }
    public string CarrierItemTypeName { get; set; }
    public string CarrierItemTypeAbbreviation { get; set; }
    
    [XmlIgnore]
    public int UnitId { get; set; }

    public void TrimAllStringFields()
    {
        Description = Description?.Trim();
        ItemType = ItemType?.Trim();
        Barcode = Barcode?.Trim();
        CarrierItemTypeName = CarrierItemTypeName?.Trim();
        CarrierItemTypeAbbreviation = CarrierItemTypeAbbreviation?.Trim();
    }
}
