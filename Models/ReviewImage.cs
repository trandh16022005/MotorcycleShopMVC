using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("review_image")]
public partial class ReviewImage
{
    [Key]
    [Column("image_id")]
    public int ImageId { get; set; }

    [Column("review_id")]
    public int ReviewId { get; set; }

    [Column("image_url")]
    [StringLength(255)]
    [Unicode(false)]
    public string ImageUrl { get; set; } = null!;

    [ForeignKey("ReviewId")]
    [InverseProperty("ReviewImages")]
    public virtual Review Review { get; set; } = null!;
}
