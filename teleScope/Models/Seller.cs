using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

[Table("sellers")]
public partial class Seller
{
    [Key]
    [Column("seller_id")]
    public int SellerId { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [Column("tax_number")]
    public int? TaxNumber { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Sellers")]
    public virtual User? User { get; set; }
}
