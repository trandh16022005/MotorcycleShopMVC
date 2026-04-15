using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("wishlist")]
public partial class Wishlist
{
    [Key]
    [Column("wishlist_id")]
    public int WishlistId { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("motorcycle_id")]
    public int? MotorcycleId { get; set; }

    [Column("part_id")]
    public int? PartId { get; set; }

    [Column("added_at", TypeName = "datetime")]
    public DateTime? AddedAt { get; set; }

    [ForeignKey("MotorcycleId")]
    [InverseProperty("Wishlists")]
    public virtual Motorcycle? Motorcycle { get; set; }

    [ForeignKey("PartId")]
    [InverseProperty("Wishlists")]
    public virtual Part? Part { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Wishlists")]
    public virtual User User { get; set; } = null!;
}
