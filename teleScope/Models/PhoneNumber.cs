using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

[Table("phone_numbers")]
[Microsoft.EntityFrameworkCore.Index("Phone", Name = "UQ__phone_nu__B43B145FA7EB9547", IsUnique = true)]
public partial class PhoneNumber
{
    [Key]
    [Column("phone_id")]
    public int PhoneId { get; set; }

    [Column("phone")]
    [StringLength(20)]
    [Unicode(false)]
    public string Phone { get; set; } = null!;

    [Column("phone_type")]
    [StringLength(10)]
    [Unicode(false)]
    [DisplayName("Phone Type")]
    public string? PhoneType { get; set; }

    [Column("program_id")]
    public int? ProgramId { get; set; }

    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Column("created_at", TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Phone")]
    public virtual ICollection<Call> Calls { get; set; } = new List<Call>();

    [ForeignKey("CustomerId")]
    [InverseProperty("PhoneNumbers")]
    public virtual Customer? Customer { get; set; }

    [ForeignKey("ProgramId")]
    [InverseProperty("PhoneNumbers")]
    public virtual Programme? Program { get; set; }
}
