using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEConETL.Entities;

[Table("customer", Schema = "dbo")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int customer_id { get; set; }

    public string? customer_name { get; set; }
    public int? customer_group_id { get; set; }
    public int? business_type_id { get; set; }
    public string? abn { get; set; }
    public string? address_name { get; set; }
    public string? address_street { get; set; }
    public string? address_suburb { get; set; }
    public string? address_state { get; set; }
    public string? address_postcode { get; set; }
    public int? branch_id { get; set; }
    public int? sales_rep_id { get; set; }
    public int? main_rep_id { get; set; }
    public int? visit_frequency_id { get; set; }
    public int? call_frequency_id { get; set; }
    public string? operational_notes { get; set; }
    public string? billing_notes { get; set; }
    public string? ops_contact_name { get; set; }
    public string? ops_contact_phone { get; set; }
    public string? ops_contact_mobile { get; set; }
    public string? ops_contact_email { get; set; }
    public string? admin_contact_name { get; set; }
    public string? admin_contact_phone { get; set; }
    public string? admin_contact_mobile { get; set; }
    public string? admin_contact_email { get; set; }
    public bool? daily_costings { get; set; }
    public int? ring_list_id { get; set; }
    public int? hourly_rate_card_id { get; set; }
    public int? default_fuel_levy_type_id { get; set; }
    public int? rounding_type_id { get; set; }
    public decimal? credit_limit { get; set; }
    public int? fixed_rate_card_id { get; set; }
    public decimal? fuellevy_metro { get; set; }
    public decimal? fuellevy_oom { get; set; }
    public int? pallet_rate_card_id { get; set; }
    public string? customer_code { get; set; }
    public bool? active { get; set; }
    public DateTime? created_at { get; set; }
    public string? created_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string? updated_by { get; set; }
}
