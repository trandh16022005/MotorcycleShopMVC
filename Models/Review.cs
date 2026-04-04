using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("review")]
public partial class Review
{
    [Key]
    [Column("review_id")]
    public int ReviewId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("order_id")]
    public int OrderId { get; set; }

    [Column("motorcycle_id")]
    public int? MotorcycleId { get; set; }

    [Column("part_id")]
    public int? PartId { get; set; }

    [Column("rating")]
    public int Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("MotorcycleId")]
    [InverseProperty("Reviews")]
    public virtual Motorcycle? Motorcycle { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Reviews")]
    public virtual Order Order { get; set; } = null!;

    [ForeignKey("PartId")]
    [InverseProperty("Reviews")]
    public virtual Part? Part { get; set; }

    [InverseProperty("Review")]
    public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

    [ForeignKey("UserId")]
    [InverseProperty("Reviews")]
    public virtual User User { get; set; } = null!;
}
