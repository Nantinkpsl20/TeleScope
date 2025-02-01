using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

[Table("calls")]
public partial class Call
{
    [Key]
    [Column("call_id")]
    public int CallId { get; set; }

    [Column("phone_id")]
    public int PhoneId { get; set; }

    [Column("call_date", TypeName = "datetime")]
    public DateTime CallDate { get; set; }

    [Column("duration")]
    public decimal Duration { get; set; }

   
    [Column("destination_number")]
    public string? DestinationNumber { get; set; }

    [Column("call_type")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CallType { get; set; }

    [Column("is_incoming")]
    public bool? IsIncoming { get; set; }

    [ForeignKey("PhoneId")]
    [InverseProperty("Calls")]
    public virtual PhoneNumber? Phone { get; set; } = null!;
}
