using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Factories;

namespace ExpenseTracker.Api.Controllers
{
    public class ExpenseGroupStatussesController : ApiController
    {
        IExpenseTrackerRepository _repository;
        ExpenseMasterDataFactory _expenseMasterDataFactory = new ExpenseMasterDataFactory();

        public ExpenseGroupStatussesController()
        {
            _repository = new ExpenseTrackerEFRepository(new Repository.Entities.ExpenseTrackerContext());
        }

        public ExpenseGroupStatussesController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
        }


        public IHttpActionResult Get()
        {

            try
            {
                // get expensegroupstatusses & map to DTO's
                var expenseGroupStatusses = _repository.GetExpenseGroupStatusses().ToList()
                    .Select(egs => _expenseMasterDataFactory.CreateExpenseGroupStatus(egs));

                return Ok(expenseGroupStatusses);

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
