using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

[Table("users")]
[Microsoft.EntityFrameworkCore.Index("Username", Name = "UQ__users__F3DBC5720946FDFB", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("user_id")]
    public int UserId { get; set; }


    [DisplayName("First Name")]
    [Column("first_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? FirstName { get; set; }


    [DisplayName("Last Name")]
    [Column("last_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string? LastName { get; set; }


    [DisplayName("Username")]
    [Column("username")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Username { get; set; }


    [DisplayName("Password")]
    [Column("password")]
    [StringLength(255)]
    [Unicode(false)]
    public string? Password { get; set; }


    [DisplayName("Email")]
    [Column("email")]
    [StringLength(100)]
    [Unicode(false)]
    public string? Email { get; set; }


    [DisplayName("Role")]
    [Column("user_role")]
    [StringLength(10)]
    [Unicode(false)]
    public string? UserRole { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Admin> Admins { get; set; } = new List<Admin>();

    [InverseProperty("User")]
    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    [InverseProperty("User")]
    public virtual ICollection<Seller> Sellers { get; set; } = new List<Seller>();
}
