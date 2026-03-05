using BankRUs.Domain.Entities;
using BankRUs.Intrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BankRUs.Intrastructure.Persistance;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(builder =>
        {
            builder
                .HasIndex(u => u.SocialSecurityNumber)
                .IsUnique();
        });

        builder.Entity<BankAccount>(builder =>
        {
            builder.Property(x => x.Balance)
              .HasPrecision(18, 2);

            builder
                .HasIndex(b => b.AccountNumber)
                .IsUnique();
        });

        builder.Entity<BankAccount>().
            HasOne<ApplicationUser>().
            WithMany().
            HasForeignKey(b => b.UserId);

       builder.Entity<BankAccountTransaction>(builder =>
       {
           
           builder.Property(x => x.Amount)
             .HasPrecision(18, 2);
           
           builder.Property(x => x.BalanceAfterTransaction)
             .HasPrecision(18, 2);
           
           builder
           .HasOne<BankAccount>()
           .WithMany()
           .HasForeignKey(t => t.BankAccountId);
       });
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankAccountTransaction> BankAccountTransactions => Set<BankAccountTransaction>();
}

