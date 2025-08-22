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
            entity.HasKey(p => p.PaymentId);
            
            entity.Property(p => p.PaymentId)
                .HasDefaultValueSql("NEWID()");
                
            entity.Property(p => p.UserId)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(p => p.StoreId)
                .HasMaxLength(50);
                
            entity.Property(p => p.CartId)
                .HasMaxLength(100);
                
            entity.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("USD");
                
            entity.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();
                
            entity.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(p => p.ItemsJson)
                .IsRequired();
                
            entity.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(p => p.ProcessedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            // Create indexes for common queries
            entity.HasIndex(p => p.UserId);
            entity.HasIndex(p => p.Status);
            entity.HasIndex(p => p.CreatedAt);
        });
    }
}