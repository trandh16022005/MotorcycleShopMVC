using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Motorcycle> Motorcycles { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<PartCategory> PartCategories { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ReviewImage> ReviewImages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MotorcycleShop;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__brand__5E5A8E27A5BB9874");
        });

        modelBuilder.Entity<Brand>().HasData(
    new Brand { BrandId = 1, BrandName = "Honda", Country = "Japan" },
    new Brand { BrandId = 2, BrandName = "Yamaha", Country = "Japan" },
    new Brand { BrandId = 3, BrandName = "Piaggio", Country = "Italy" },
    new Brand { BrandId = 4, BrandName = "Suzuki", Country = "Japan" },
    new Brand { BrandId = 5, BrandName = "SYM", Country = "Taiwan" },
    new Brand { BrandId = 6, BrandName = "Kymco", Country = "Taiwan" },
    new Brand { BrandId = 7, BrandName = "Kawasaki", Country = "Japan" },
    new Brand { BrandId = 8, BrandName = "Detech", Country = "Vietnam" },
    new Brand { BrandId = 9, BrandName = "Halim", Country = "Vietnam" },
    new Brand { BrandId = 10, BrandName = "Daelim", Country = "South Korea" }
);

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__cart__2EF52A270BAE5CCE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithOne(p => p.Cart).HasConstraintName("FK__cart__user_id__6B24EA82");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__cart_ite__5D9A6C6EA928553F");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK__cart_item__cart___6EF57B66");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.CartItems).HasConstraintName("FK__cart_item__motor__6FE99F9F");

            entity.HasOne(d => d.Part).WithMany(p => p.CartItems).HasConstraintName("FK__cart_item__part___70DDC3D8");
        });

        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.HasKey(e => e.MotorcycleId).HasName("PK__motorcyc__7340036BC8A4541C");

            entity.HasOne(d => d.Brand).WithMany(p => p.Motorcycles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__motorcycl__brand__5812160E");

            entity.HasOne(d => d.Type).WithMany(p => p.Motorcycles).HasConstraintName("FK__motorcycl__type___59063A47");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842FB0452DD2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasConstraintName("FK__notificat__user___1CBC4616");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__order__46596229E8CFF554");

            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderStatus).HasDefaultValue("Processing");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Pending");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order__user_id__797309D9");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__order_it__3764B6BCA50C8FC5");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__motor__7D439ABD");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__order__7C4F7684");

            entity.HasOne(d => d.Part).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__part___7E37BEF6");
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__part__A0E3FAB80A22E3EB");

            entity.Property(e => e.WarrantyMonths).HasDefaultValue(0);

            entity.HasOne(d => d.Brand).WithMany(p => p.Parts).HasConstraintName("FK__part__brand_id__619B8048");

            entity.HasOne(d => d.Category).WithMany(p => p.Parts).HasConstraintName("FK__part__category_i__60A75C0F");

            entity.HasMany(d => d.Motorcycles).WithMany(p => p.Parts)
                .UsingEntity<Dictionary<string, object>>(
                    "PartMotorcycle",
                    r => r.HasOne<Motorcycle>().WithMany()
                        .HasForeignKey("MotorcycleId")
                        .HasConstraintName("FK__part_moto__motor__656C112C"),
                    l => l.HasOne<Part>().WithMany()
                        .HasForeignKey("PartId")
                        .HasConstraintName("FK__part_moto__part___6477ECF3"),
                    j =>
                    {
                        j.HasKey("PartId", "MotorcycleId").HasName("PK__part_mot__07D7FA8E704CCD33");
                        j.ToTable("part_motorcycle");
                        j.IndexerProperty<int>("PartId").HasColumnName("part_id");
                        j.IndexerProperty<int>("MotorcycleId").HasColumnName("motorcycle_id");
                    });
        });

        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__part_cat__D54EE9B443779948");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__promotio__2CB9556BB4BBD7AF");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MinOrderValue).HasDefaultValue(0m);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__review__60883D900AB2ADDB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.Reviews).HasConstraintName("FK__review__motorcyc__05D8E0BE");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__order_id__04E4BC85");

            entity.HasOne(d => d.Part).WithMany(p => p.Reviews).HasConstraintName("FK__review__part_id__06CD04F7");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__user_id__03F0984C");
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__review_i__DC9AC9556E361591");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages).HasConstraintName("FK__review_im__revie__0A9D95DB");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F96AB5311");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("customer");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__vehicle___2C0005983B1A1B59");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__wishlist__6151514E4074E732");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__motorc__160F4887");

            entity.HasOne(d => d.Part).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__part_i__17036CC0");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__user_i__151B244E");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
