using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("part")]
public partial class Part
{
    [Key]
    [Column("part_id")]
    public int PartId { get; set; }

    [Column("part_name")]
    [StringLength(150)]
    public string PartName { get; set; } = null!;

    [Column("category_id")]
    public int? CategoryId { get; set; }

    [Column("brand_id")]
    public int? BrandId { get; set; }

    [Column("price", TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column("stock_quantity")]
    public int StockQuantity { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("image_url")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [Column("warranty_months")]
    public int? WarrantyMonths { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Parts")]
    public virtual Brand? Brand { get; set; }

    [InverseProperty("Part")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [ForeignKey("CategoryId")]
    [InverseProperty("Parts")]
    public virtual PartCategory? Category { get; set; }

    [InverseProperty("Part")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Part")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Part")]
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    [ForeignKey("PartId")]
    [InverseProperty("Parts")]
    public virtual ICollection<Motorcycle> Motorcycles { get; set; } = new List<Motorcycle>();
}
