using Microsoft.EntityFrameworkCore;
using muniflow.api.Models;

namespace muniflow.api.Data
{
    public class MuniFlowDbContext : DbContext
    {
        public MuniFlowDbContext(DbContextOptions<MuniFlowDbContext> options)
            : base(options)
        {
        }

        // DbSet properties - one for each table
        public DbSet<Department> Departments { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<Approval> Approvals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Department -> PurchaseOrder relationship
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Department)
                .WithMany(d => d.PurchaseOrders)
                .HasForeignKey(po => po.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PurchaseOrder -> Approval relationship
            modelBuilder.Entity<Approval>()
                .HasOne(a => a.PurchaseOrder)
                .WithMany(po => po.Approvals)
                .HasForeignKey(a => a.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial Department data
            modelBuilder.Entity<Department>().HasData(
                new Department
                {
                    DepartmentId = 1,
                    Name = "Finance",
                    Description = "Financial operations and accounting",
                    AnnualBudget = 500000m,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Department
                {
                    DepartmentId = 2,
                    Name = "IT",
                    Description = "Information Technology and Innovation Services",
                    AnnualBudget = 750000m,
                    CreatedAt = new DateTime(2026, 1, 1)
                },
                new Department
                {
                    DepartmentId = 3,
                    Name = "Public Works",
                    Description = "Roads, drainage, and public infrastructure",
                    AnnualBudget = 1200000m,
                    CreatedAt = new DateTime(2026, 1, 1)
                }
            );
        }
    }
}