using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace MotorcycleShopMVC.Models;

[Table("motorcycle")]
public partial class Motorcycle
{
    [Key]
    [Column("motorcycle_id")]
    public int MotorcycleId { get; set; }

    [Column("model_name")]
    [StringLength(100)]
    public string ModelName { get; set; } = null!;

    [Column("brand_id")]
    public int BrandId { get; set; }

    [Column("type_id")]
    public int? TypeId { get; set; }

    [Column("engine_capacity")]
    public int? EngineCapacity { get; set; }

    [Column("year_from")]
    public int? YearFrom { get; set; }

    [Column("year_to")]
    public int? YearTo { get; set; }

    [Column("price", TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }

    [Column("color")]
    [StringLength(50)]
    public string? Color { get; set; }

    [Column("warranty_policy")]
    [StringLength(255)]
    public string? WarrantyPolicy { get; set; }

    [Column("image_url")]
    [StringLength(255)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    [Column("description")]
    public string? Description { get; set; }

    [Column("stock_qty")]
    public int StockQty { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Motorcycles")]
    public virtual Brand Brand { get; set; } = null!;

    [InverseProperty("Motorcycle")]
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    [InverseProperty("Motorcycle")]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    [InverseProperty("Motorcycle")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("TypeId")]
    [InverseProperty("Motorcycles")]
    public virtual VehicleType? Type { get; set; }

    [InverseProperty("Motorcycle")]
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    [ForeignKey("MotorcycleId")]
    [InverseProperty("Motorcycles")]
    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}
