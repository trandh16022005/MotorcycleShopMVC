using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("cart_item")]
public partial class CartItem
{
    [Key]
    [Column("cart_item_id")]
    public int CartItemId { get; set; }

    [Column("cart_id")]
    public int CartId { get; set; }

    [Column("motorcycle_id")]
    public int? MotorcycleId { get; set; }

    [Column("part_id")]
    public int? PartId { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [ForeignKey("CartId")]
    [InverseProperty("CartItems")]
    public virtual Cart Cart { get; set; } = null!;

    [ForeignKey("MotorcycleId")]
    [InverseProperty("CartItems")]
    public virtual Motorcycle? Motorcycle { get; set; }

    [ForeignKey("PartId")]
    [InverseProperty("CartItems")]
    public virtual Part? Part { get; set; }
}
