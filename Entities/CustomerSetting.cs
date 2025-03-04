using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEConETL.Entities;

[Table("customer_setting", Schema = "dbo")]
public class CustomerSetting
{
    [Key]
    public int customer_id { get; set; }

    public bool? enable_consignment { get; set; }
    public int? billing_template_id { get; set; }
    public DateTime created_at { get; set; }
    public string? created_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string? updated_by { get; set; }
}