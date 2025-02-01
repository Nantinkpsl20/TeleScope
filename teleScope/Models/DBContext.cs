using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

public partial class DBContext : DbContext
{
    public DBContext()
    {
    }

    public DBContext(DbContextOptions<DBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<Call> Calls { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<PhoneNumber> PhoneNumbers { get; set; }

    public virtual DbSet<Programme> Programmes { get; set; }

    public virtual DbSet<Seller> Sellers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAPTOP-ELQ753VH\\SQLEXPRESS;Database=db_telescope;Trusted_Connection=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__admins__43AA4141E46A716E");

            entity.HasOne(d => d.User).WithMany(p => p.Admins)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__admins__user_id__7A672E12");
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__bills__D706DDB3B5F5211F");

            entity.Property(e => e.IsPaid).HasDefaultValue(false);

            entity.HasOne(d => d.Customer).WithMany(p => p.Bills)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__bills__customer___0D7A0286");
        });

        modelBuilder.Entity<Call>(entity =>
        {
            entity.HasKey(e => e.CallId).HasName("PK__calls__427DCE683B27BFD2");

            entity.Property(e => e.CallType).HasDefaultValue("local");
            entity.Property(e => e.IsIncoming).HasDefaultValue(false);

            entity.HasOne(d => d.Phone).WithMany(p => p.Calls).HasConstraintName("FK__calls__phone_id__09A971A2");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB852FB77121");

            entity.HasOne(d => d.User).WithMany(p => p.Customers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__customers__user___778AC167");
        });

        modelBuilder.Entity<PhoneNumber>(entity =>
        {
            entity.HasKey(e => e.PhoneId).HasName("PK__phone_nu__E6BD6DD7BBE1D578");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.PhoneNumbers).HasConstraintName("FK__phone_num__custo__02FC7413");

            entity.HasOne(d => d.Program).WithMany(p => p.PhoneNumbers)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__phone_num__progr__03F0984C");
        });

        modelBuilder.Entity<Programme>(entity =>
        {
            entity.HasKey(e => e.ProgramId).HasName("PK__programm__3A7890AC8387909C");
        });

        modelBuilder.Entity<Seller>(entity =>
        {
            entity.HasKey(e => e.SellerId).HasName("PK__sellers__780A0A97C0100223");

            entity.HasOne(d => d.User).WithMany(p => p.Sellers)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__sellers__user_id__74AE54BC");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FF26EDAB7");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
