using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MotorcycleShopMVC.Models;

[Table("vehicle_type")]
[Index("TypeName", Name = "UQ__vehicle___543C4FD973271E93", IsUnique = true)]
public partial class VehicleType
{
    [Key]
    [Column("type_id")]
    public int TypeId { get; set; }

    [Column("type_name")]
    [StringLength(50)]
    public string TypeName { get; set; } = null!;

    [InverseProperty("Type")]
    public virtual ICollection<Motorcycle> Motorcycles { get; set; } = new List<Motorcycle>();
}
