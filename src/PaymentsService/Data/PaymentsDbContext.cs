using Microsoft.EntityFrameworkCore;
using PaymentsService.Models;

namespace PaymentsService.Data;

public class PaymentsDbContext : DbContext
{
    public PaymentsDbContext(DbContextOptions<PaymentsDbContext> options) : base(options)
    {
    }
    
    public DbSet<PaymentRecord> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure PaymentRecord entity
        modelBuilder.Entity<PaymentRecord>(entity =>
        {
            entity.HasKey(e => e.PaymentId);
            entity.Property(e => e.PaymentId)
                .IsRequired()
                .ValueGeneratedOnAdd();
                
            entity.Property(e => e.UserId)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.StoreId)
                .HasMaxLength(255);
                
            entity.Property(e => e.CartId)
                .HasMaxLength(255);
                
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(10);
                
            entity.Property(e => e.Amount)
                .IsRequired()
                .HasPrecision(18, 2);
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.PaymentMethod)
                .IsRequired()
                .HasMaxLength(255);
                
            entity.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            entity.Property(e => e.ProcessedAt)
                .IsRequired()
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
                
            // Index for queries
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Status);
        });
    }
}