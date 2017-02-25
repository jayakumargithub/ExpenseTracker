using ExpenseTracker.Dto;
using ExpenseTracker.WebClient.Helpers;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTracker.WebClient.Models
{
    public class ExpenseGroupsViewModel
    {
        public IPagedList<Dto.ExpenseGroup> ExpenseGroups { get; set; }

        public IEnumerable<Dto.ExpenseGroupStatus> ExpenseGroupStatusses { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}