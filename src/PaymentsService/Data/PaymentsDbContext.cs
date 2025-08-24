using Microsoft.EntityFrameworkCore;

namespace PaymentsService.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
    {
    }

    public DbSet<Models.PaymentRecord> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure decimal precision for Amount
        modelBuilder.Entity<Models.PaymentRecord>()
            .Property(p => p.Amount)
            .HasPrecision(18, 2);

        // Configure indexes for common queries
        modelBuilder.Entity<Models.PaymentRecord>()
            .HasIndex(p => p.UserId);

        modelBuilder.Entity<Models.PaymentRecord>()
            .HasIndex(p => p.Status);

        modelBuilder.Entity<Models.PaymentRecord>()
            .HasIndex(p => p.CreatedAt);
    }
}