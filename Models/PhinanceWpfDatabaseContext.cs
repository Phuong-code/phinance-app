using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace phinance2.Models;

public partial class PhinanceWpfDatabaseContext : DbContext
{
    public PhinanceWpfDatabaseContext()
    {
    }

    public PhinanceWpfDatabaseContext(DbContextOptions<PhinanceWpfDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AccountBalance> AccountBalances { get; set; }

    public virtual DbSet<CryptoPrice> CryptoPrices { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost; uid=Phil; pwd=Sieunhan123; database=phinance_wpf_database");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AccountBalance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("account_balance");

            entity.HasIndex(e => e.UserId, "user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bnb)
                .HasDefaultValueSql("'0.00000000'")
                .HasColumnName("bnb");
            entity.Property(e => e.Btc)
                .HasDefaultValueSql("'0.00000000'")
                .HasColumnName("btc");
            entity.Property(e => e.Doge)
                .HasDefaultValueSql("'0.00000000'")
                .HasColumnName("doge");
            entity.Property(e => e.Eth)
                .HasDefaultValueSql("'0.00000000'")
                .HasColumnName("eth");
            entity.Property(e => e.Usd)
                .HasDefaultValueSql("'0.00000000'")
                .HasColumnName("usd");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Xrp)
                .HasDefaultValueSql("'0.00000000'")
                .HasColumnName("xrp");

            entity.HasOne(d => d.User).WithMany(p => p.AccountBalances)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("account_balance_ibfk_1");
        });

        modelBuilder.Entity<CryptoPrice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("crypto_price");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Symbol)
                .HasMaxLength(10)
                .HasColumnName("symbol");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("user");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
