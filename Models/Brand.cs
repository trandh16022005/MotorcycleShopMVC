using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("brand")]
[Index("BrandName", Name = "UQ__brand__0C0C3B58AC12B969", IsUnique = true)]
public partial class Brand
{
    [Key]
    [Column("brand_id")]
    public int BrandId { get; set; }

    [Column("brand_name")]
    [StringLength(100)]
    public string BrandName { get; set; } = null!;

    [Column("country")]
    [StringLength(100)]
    public string? Country { get; set; }

    [InverseProperty("Brand")]
    public virtual ICollection<Motorcycle> Motorcycles { get; set; } = new List<Motorcycle>();

    [InverseProperty("Brand")]
    public virtual ICollection<Part> Parts { get; set; } = new List<Part>();
}
