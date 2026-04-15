using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("promotion")]
[Index("Code", Name = "UQ__promotio__357D4CF9CDF8E738", IsUnique = true)]
public partial class Promotion
{
    [Key]
    [Column("promotion_id")]
    public int PromotionId { get; set; }

    [Column("code")]
    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("discount_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string DiscountType { get; set; } = null!;

    [Column("discount_value", TypeName = "decimal(18, 2)")]
    public decimal DiscountValue { get; set; }

    [Column("min_order_value", TypeName = "decimal(18, 2)")]
    public decimal? MinOrderValue { get; set; }

    [Column("start_date", TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column("end_date", TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    [Column("is_active")]
    public bool? IsActive { get; set; }
}
