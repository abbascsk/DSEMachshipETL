using DSEConETL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DSEConETL.Data;

public class DseDbContext(DbContextOptions<DseDbContext> options) : DbContext(options)
{
    public DbSet<Consignment> Consignments { get; set; }
    public DbSet<ConsignmentItem> ConsignmentItems { get; set; }
}