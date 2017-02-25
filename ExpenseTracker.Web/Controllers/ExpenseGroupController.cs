using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExpenseTracker.Dto;
using ExpenseTracker.Web.Helper;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ExpenseTracker.Web.Models;
using System.Net;

namespace ExpenseTracker.Web.Controllers
{
    public class ExpenseGroupController : Controller
    {
        // GET: ExpenseGroup
        public async Task<ActionResult> Index()

        {
            var client = ExpenseTrackerHttpClient.GetClient();
            var model = new ExpenseGroupViewModel();

            var egsResponse = await client.GetAsync("api/ExpenseGroupStatusses");
            if (egsResponse.IsSuccessStatusCode)
            {
                string egsContent = await egsResponse.Content.ReadAsStringAsync();
                var lstExpenseGroupStatus = JsonConvert.DeserializeObject<IEnumerable<ExpenseGroupStatus>>(egsContent);
                model.ExpenseGroupStatuses = lstExpenseGroupStatus;
            }
            else
            {
                return Content("An error occured");
            }

            HttpResponseMessage response = await client.GetAsync("api/expensesgroup");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var listExpenseGroup= JsonConvert.DeserializeObject<IEnumerable<ExpenseGroup>>(content);
                model.ExpenseGroup = listExpenseGroup;
            }
            else
            {
                return Content("The error occured");
            }

            return View(model);
        }

        public ActionResult Details(int id)
        {
            return Content("Not implemented yet");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ExpenseGroup expenseGroup)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();
                expenseGroup.ExpenseGroupStatusId = 1;
                expenseGroup.UserId = "http://expensetrackeridsrv3/embedded_1";
                //expensegrouppost
                var serializeItemToCreate = JsonConvert.SerializeObject(expenseGroup);
                //var response = await client.PostAsync("api/")

                var response = await client.PostAsync("api/expensesgroup", new StringContent(serializeItemToCreate, System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("An error occurred");
                }

            }
            catch(Exception e)
            {
                return Content("An error occurred.");
            }
            
        }

        public async  Task<ActionResult> Edit(int id)
        {

            var client = ExpenseTrackerHttpClient.GetClient();
            HttpResponseMessage response = await client.GetAsync("api/expensesgroup/" + id);
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<Dto.ExpenseGroup>(content);
                return View(model);

            }
            return Content("an error occured");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ExpenseGroup expenseGroup)
        {
            try
            {
                var client = ExpenseTrackerHttpClient.GetClient();
                expenseGroup.ExpenseGroupStatusId = 1;
                expenseGroup.UserId = "http://expensetrackeridsrv3/embedded_1";
                var serializeItemToUpdate = JsonConvert.SerializeObject(expenseGroup);
                var response = await client.PutAsync("api/expensesgroup/" + id, new StringContent(serializeItemToUpdate, System.Text.Encoding.Unicode, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("an error occured");
                }
            }
            catch(Exception ex)
            {
                return Content("An error occured");
            }
        }

    }
}