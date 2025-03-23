using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEMachshipETL.Entities;

[Table("pallet_type", Schema = "dbo")]
public partial class PalletType
{
    [Key]
    public int pallet_type_id { get; set; }
    public string? pallet_type_name { get; set; }
    public string? pallet_type_abbreviation { get; set; }
    public int? sort_order { get; set; }
    public DateTime? created_on { get; set; }
    public string? created_by { get; set; }
    public DateTime? updated_on { get; set; }
    public string? updated_by { get; set; }
}