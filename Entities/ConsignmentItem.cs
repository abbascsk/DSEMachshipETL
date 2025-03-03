using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEConETL.Entities;

[Table("consignment_item", Schema = "dbo")]
public partial class ConsignmentItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int consignment_item_id { get; set; }
    public int consignment_id { get; set; }
    public int? unit_id { get; set; }
    public int? line_no { get; set; }
    public string item_description { get; set; }
    public decimal? weight { get; set; }
    public int? length { get; set; }
    public int? width { get; set; }
    public int? height { get; set; }
    public string sku { get; set; }
    public string item_reference { get; set; }
    public DateTime created_at { get; set; }
    public string created_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string updated_by { get; set; }
    public string barcode { get; set; }
    public bool? stackable { get; set; }
    public bool? reorient { get; set; }
}