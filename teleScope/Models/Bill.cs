using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace teleScope.Models;

[Table("bills")]
public partial class Bill
{
    [Key]
    [Column("bill_id")]
    public int BillId { get; set; }

    [DisplayName("Username")]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [DisplayName("Issue Date")]
    [Column("issue_date", TypeName = "datetime")]
    public DateTime IssueDate { get; set; }

    [DisplayName("Due Date")]
    [Column("due_date", TypeName = "datetime")]
    public DateTime DueDate { get; set; }

    [DisplayName("Total Amount")]
    [Column("total_amount", TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [DisplayName("Status")]
    [Column("is_paid")]
    public bool? IsPaid { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Bills")]
    public virtual Customer? Customer { get; set; }
}
