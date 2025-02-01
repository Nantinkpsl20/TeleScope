using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

[Table("programmes")]
[Microsoft.EntityFrameworkCore.Index("ProgrName", Name = "UQ__programm__46AE24AE7D6C7111", IsUnique = true)]
public partial class Programme
{
    [Key]
    [Column("program_id")]
    public int ProgramId { get; set; }

    [Column("progr_name")]
    [StringLength(100)]
    [Unicode(false)]
    [DisplayName("Program Name")]
    public string? ProgrName { get; set; }

    [DisplayName("Landline Minutes")]
    [Column("landline_minutes")]
    public int LandlineMinutes { get; set; }

    [DisplayName("Mobile Minutes")]
    [Column("mobile_minutes")]
    public int MobileMinutes { get; set; }

    [DisplayName("Program Cost")]
    [Column("fixed_cost", TypeName = "decimal(10, 2)")]
    public decimal FixedCost { get; set; }

    [DisplayName("Landline Fee")]
    [Column("landline_fee", TypeName = "decimal(10, 2)")]
    public decimal LandlineFee { get; set; }

    [DisplayName("Mobile Fee")]
    [Column("mobile_fee", TypeName = "decimal(10, 2)")]
    public decimal MobileFee { get; set; }

    [DisplayName("Five Digit Fee")]
    [Column("five_digit_fee", TypeName = "decimal(10, 2)")]
    public decimal FiveDigitFee { get; set; }

    [InverseProperty("Program")]
    public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; } = new List<PhoneNumber>();
}
