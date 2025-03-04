using DSEConETL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DSEConETL.Data;

public class DseDbContext(DbContextOptions<DseDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerSetting> CustomerSettings { get; set; }
    public DbSet<ConsignmentServiceType> ConsignmentServiceTypes { get; set; }
    public DbSet<PalletType> PalletTypes { get; set; }
    public DbSet<PalletExchangeType> PalletExchangeTypes { get; set; }
    public DbSet<Unit> Units { get; set; }
    public DbSet<Consignment> Consignments { get; set; }
    public DbSet<ConsignmentItem> ConsignmentItems { get; set; }
}