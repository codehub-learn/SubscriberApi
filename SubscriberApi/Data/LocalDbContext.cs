using System.Reflection.Emit;

namespace SubscriberApi.Data;
using Microsoft.EntityFrameworkCore;
using SubscriberApi.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Loan> Loans => Set<Loan>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.ToTable("Loans");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.BorrowerName)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(x => x.Amount)
                  .HasColumnType("decimal(18,2)");

            entity.Property(x => x.CreatedAtUtc)
                  .IsRequired();
        });
    }
}
