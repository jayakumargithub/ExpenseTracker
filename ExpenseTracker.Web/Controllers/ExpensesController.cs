using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExpenseTracker.Dto;

namespace ExpenseTracker.Web.Controllers
{
    public class ExpensesController : Controller
    {
        // GET: Expenses
        public ActionResult Index()
        {
            return View(new List<ExpenseGroup>());
        }

        public ActionResult Details(int id)
        {
            return Content("Not yet implemented");
        }

        public ActionResult Create()
        {
            return View();
        }
    }
}