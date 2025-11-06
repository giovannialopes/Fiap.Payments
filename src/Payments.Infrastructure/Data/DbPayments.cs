using Microsoft.EntityFrameworkCore;
using Payments.Domain.Entities;
using System.Text.Json;

namespace Payments.Infrastructure.Data;

public class DbPayments : DbContext
{
    public DbPayments(DbContextOptions<DbPayments> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PaymentsEnt>().ToTable("PAGAMENTOS");
        modelBuilder.Entity<ILoggerEnt>().ToTable("LOGS");
    }

    public DbSet<PaymentsEnt> PAGAMENTOS { get; set; }
    public DbSet<ILoggerEnt> LOGS { get; set; }
}
