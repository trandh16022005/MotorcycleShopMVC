using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
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
        => optionsBuilder.UseSqlServer("Server=localhost;Database=MotorcycleShop;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.BrandId).HasName("PK__brand__5E5A8E27D6FBE891");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__cart__2EF52A2736B8EABB");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithOne(p => p.Cart).HasConstraintName("FK__cart__user_id__6FE99F9F");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__cart_ite__5D9A6C6ED6140EC7");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems).HasConstraintName("FK__cart_item__cart___73BA3083");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.CartItems).HasConstraintName("FK__cart_item__motor__74AE54BC");

            entity.HasOne(d => d.Part).WithMany(p => p.CartItems).HasConstraintName("FK__cart_item__part___75A278F5");
        });

        modelBuilder.Entity<Motorcycle>(entity =>
        {
            entity.HasKey(e => e.MotorcycleId).HasName("PK__motorcyc__7340036B1F7C9B32");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Brand).WithMany(p => p.Motorcycles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__motorcycl__brand__5AEE82B9");

            entity.HasOne(d => d.Type).WithMany(p => p.Motorcycles).HasConstraintName("FK__motorcycl__type___5BE2A6F2");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__notifica__E059842F23D1F62F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications).HasConstraintName("FK__notificat__user___22751F6C");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__order__4659622989CB449D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.OrderStatus).HasDefaultValue("Processing");
            entity.Property(e => e.PaymentStatus).HasDefaultValue("Pending");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__order__user_id__00200768");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.OrderItemId).HasName("PK__order_it__3764B6BC2A1FACB9");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__motor__03F0984C");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__order__02FC7413");

            entity.HasOne(d => d.Part).WithMany(p => p.OrderItems).HasConstraintName("FK__order_ite__part___04E4BC85");
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__part__A0E3FAB8AE85F9B9");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.WarrantyMonths).HasDefaultValue(0);

            entity.HasOne(d => d.Brand).WithMany(p => p.Parts).HasConstraintName("FK__part__brand_id__66603565");

            entity.HasOne(d => d.Category).WithMany(p => p.Parts).HasConstraintName("FK__part__category_i__656C112C");

            entity.HasMany(d => d.Motorcycles).WithMany(p => p.Parts)
                .UsingEntity<Dictionary<string, object>>(
                    "PartMotorcycle",
                    r => r.HasOne<Motorcycle>().WithMany()
                        .HasForeignKey("MotorcycleId")
                        .HasConstraintName("FK__part_moto__motor__6A30C649"),
                    l => l.HasOne<Part>().WithMany()
                        .HasForeignKey("PartId")
                        .HasConstraintName("FK__part_moto__part___693CA210"),
                    j =>
                    {
                        j.HasKey("PartId", "MotorcycleId").HasName("PK__part_mot__07D7FA8E3F4FEAC2");
                        j.ToTable("part_motorcycle");
                        j.IndexerProperty<int>("PartId").HasColumnName("part_id");
                        j.IndexerProperty<int>("MotorcycleId").HasColumnName("motorcycle_id");
                    });
        });

        modelBuilder.Entity<PartCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__part_cat__D54EE9B4BB9F450B");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.PromotionId).HasName("PK__promotio__2CB9556B251A8A0D");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MinOrderValue).HasDefaultValue(0m);
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__review__60883D90AA4CCE2F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.Reviews).HasConstraintName("FK__review__motorcyc__0C85DE4D");

            entity.HasOne(d => d.Order).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__order_id__0B91BA14");

            entity.HasOne(d => d.Part).WithMany(p => p.Reviews).HasConstraintName("FK__review__part_id__0D7A0286");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__review__user_id__0A9D95DB");
        });

        modelBuilder.Entity<ReviewImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__review_i__DC9AC955651A6002");

            entity.HasOne(d => d.Review).WithMany(p => p.ReviewImages).HasConstraintName("FK__review_im__revie__114A936A");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F3F46CAE1");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Role).HasDefaultValue("customer");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.TypeId).HasName("PK__vehicle___2C000598AB825BDC");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__wishlist__6151514E992593C9");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Motorcycle).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__motorc__1BC821DD");

            entity.HasOne(d => d.Part).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__part_i__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists).HasConstraintName("FK__wishlist__user_i__1AD3FDA4");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
