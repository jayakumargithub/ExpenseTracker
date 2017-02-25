using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExpenseTracker.Dto;

namespace ExpenseTracker.Web.Models
{
    public class ExpenseGroupViewModel
    {
        public IEnumerable<ExpenseGroup> ExpenseGroup { get; set; }
        public IEnumerable<ExpenseGroupStatus> ExpenseGroupStatuses { get; set; }
    }
}