using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using PagedList.Mvc;
using ExpenseTracker.Web.Helper;

namespace ExpenseTracker.Web.Models
{
    public class ExpenseGroupsViewModel1
    {
        public IPagedList<Dto.ExpenseGroup> ExpenseGroups { get; set; }

        public IEnumerable<Dto.ExpenseGroupStatus> ExpenseGroupStatusses { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}