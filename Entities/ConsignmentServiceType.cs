using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEConETL.Entities;

[Table("consignment_service_type", Schema = "dbo")]
public class ConsignmentServiceType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int consignment_service_type_id { get; set; }

    public string? consignment_service_type_name { get; set; }
    public string? consignment_service_type_abbreviation { get; set; }
    public int? sort_order { get; set; }
    public DateTime created_at { get; set; }
    public string? created_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string? updated_by { get; set; }
}