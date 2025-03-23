using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSEMachshipETL.Entities;

[Table("branch", Schema = "dbo")]
public partial class Branch
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int branch_id { get; set; }
    public string? branch_name { get; set; }
    public string? branch_abbreviation { get; set; }
    public int? sort_order { get; set; }
    public decimal? default_payroll_cost { get; set; }
    public decimal? default_other_cost { get; set; }
    public decimal? default_tax_cost { get; set; }
    public string? cost_centre { get; set; }
    public string? api_notification_email { get; set; }
    public DateTime? created_on { get; set; }
    public string? created_by { get; set; }
    public DateTime? updated_on { get; set; }
    public string? updated_by { get; set; }
}
