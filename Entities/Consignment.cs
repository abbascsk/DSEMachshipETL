using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEConETL.Entities;

[Table("consignment", Schema = "dbo")]
public class Consignment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int consignment_id { get; set; }
    public int? consignment_service_type_id { get; set; }
    public int customer_id { get; set; }
    public string customer_account_no { get; set; }
    public string consignment_instructions { get; set; }
    public DateTime? dispatch_date { get; set; }
    public string sender_name { get; set; }
    public string sender_street { get; set; }
    public string sender_suburb { get; set; }
    public string sender_state { get; set; }
    public string sender_postcode { get; set; }
    public string sender_country { get; set; }
    public string sender_email { get; set; }
    public string sender_contact { get; set; }
    public string sender_phone { get; set; }
    public string sender_mobile { get; set; }
    public string sender_site_info { get; set; }
    public string sender_ref { get; set; }
    public string pickup_instructions { get; set; }
    public DateTime? pickup_start_time { get; set; }
    public DateTime? pickup_end_time { get; set; }
    public string receiver_name { get; set; }
    public string receiver_street { get; set; }
    public string receiver_suburb { get; set; }
    public string receiver_state { get; set; }
    public string receiver_postcode { get; set; }
    public string receiver_country { get; set; }
    public string receiver_email { get; set; }
    public string receiver_contact { get; set; }
    public string receiver_phone { get; set; }
    public string receiver_mobile { get; set; }
    public string receiver_site_info { get; set; }
    public string receiver_ref { get; set; }
    public string delivery_instructions { get; set; }
    public DateTime? delivery_start_time { get; set; }
    public DateTime? delivery_end_time { get; set; }
    public int? split_consignment_id { get; set; }
    public string consignment_no_prefix { get; set; }
    public int? consignment_no { get; set; }
    public int consignment_status_type_id { get; set; }
    public DateTime? batch_date { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public string consignment_no_full { get; set; }

    public int? billing_template_id { get; set; }
    public bool? dangerous_goods { get; set; }
    public bool? tail_lift { get; set; }
    public bool? atl { get; set; }
    public bool? hand_unload { get; set; }
    public bool? two_men_required { get; set; }
    public int? pallet_type_id { get; set; }
    public int? pallet_exchange_type_id { get; set; }
    public int? service_type_id { get; set; }
    public int? service_multiplier { get; set; }
    public int? from_zone_id { get; set; }
    public int? to_zone_id { get; set; }
    public string bill_number { get; set; }
    public decimal? charge_fuel_levy { get; set; }
    public decimal? charge_rate { get; set; }
    public decimal? total_charge { get; set; }
    public string invoice_comments { get; set; }
    public int? onforwarder_supplier_id { get; set; }
    public string edi_id { get; set; }
    public DateTime created_at { get; set; }
    public string created_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string updated_by { get; set; }
}