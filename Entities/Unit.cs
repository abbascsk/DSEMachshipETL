using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEMachshipETL.Entities;

[Table("unit", Schema = "dbo")]
public class Unit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int unit_id { get; set; }

    public string? unit_name { get; set; }
    public string? unit_short_name { get; set; }
    public DateTime created_at { get; set; }
    public string? created_by { get; set; }
    public DateTime? updated_at { get; set; }
    public string? updated_by { get; set; }
}