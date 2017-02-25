﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpenseTracker.Dto;

namespace ExpenseTracker.Repository.Entities
{
    [Table("Expense")]
    public partial class Expense
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        public int ExpenseGroupId { get; set; }

        public virtual ExpenseGroup ExpenseGroup { get; set; }

    }
}
