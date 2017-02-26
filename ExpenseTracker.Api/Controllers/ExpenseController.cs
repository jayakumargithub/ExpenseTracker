using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ExpenseTracker.Api.AutoMapper;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Api.Controllers
{
    [RoutePrefix("api")]
    public class ExpenseController : ApiController
    {
        private readonly IExpenseTrackerRepository _repository;
        IMapper mapper;
        private ExepenseGroupFactory _exepenseGroupFactory;
        public ExpenseController(IExpenseTrackerRepository repository)
        {
            mapper = Mapping.Configuration();
            _repository = repository;
            _exepenseGroupFactory = new ExepenseGroupFactory();
        }

        //http://localhost:51825/api/expensegroup/1/expense
        [Route("expenseforgroup/{expenseGroupId}/expense", Name = "ExpenseForGroup")]
        public IHttpActionResult Get(int expenseGroupId)
        {
            try
            {
                var expenses = _repository.GetExpenses(expenseGroupId);
                if (expenses == null)
                {
                    return NotFound();
                }


                var expenseResult = expenses.ToList().Select((exp => mapper.Map<Dto.Expense>(exp)));
                return Ok(expenseResult);
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("expenses/{expenseGroupId}")] 
        public IHttpActionResult GetByGroupId(int expenseGroupId)
        {
            IQueryable<Expense> expenseQuery = null;
            try
            {
                Repository.Entities.Expense expense = null;
                expenseQuery = _repository.GetExpenses(expenseGroupId);
                var output = expenseQuery.ToList().Select((exp => mapper.Map<Dto.Expense>(exp)));
                return Ok(output);

            }
            catch (Exception ex)
            {
                return InternalServerError();
            }


            return null;
        }

        //http://localhost:51825/api/expensegroup/1/expense/1
        [Route("expensegroup/{expenseGroupId}/expense/{id}")]
        [Route("expense/{id}")]
        public IHttpActionResult Get(int id, int? expenseGroupId)
        {
            try
            {
                Repository.Entities.Expense expense = null;
                if (expenseGroupId == null)
                {
                    expense = _repository.GetExpense(id);
                }
                else
                {
                    var expensesForGroup = _repository.GetExpenses((int)expenseGroupId);
                    if (expensesForGroup != null)
                    {
                        expense = expensesForGroup.FirstOrDefault(eg => eg.Id == id);
                    }
                }

                if (expense != null)
                {
                    var returnValue = mapper.Map<Dto.Expense>(expense);
                    return Ok(returnValue);
                }
                else
                {
                    return NotFound();
                }

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("expensewithfields/{expenseGroupId}/expense", Name = "ExpenseWithFields")]
        public IHttpActionResult Get(int expenseGroupId, string fields = null)
        {
            try
            {

                List<string> listOfFields = new List<string>();



                if (fields != null)
                {
                    listOfFields = fields.ToLower().Split(',').ToList();
                }
                var expenses = _repository.GetExpenses(expenseGroupId);
                if (expenses == null)
                {
                    return NotFound();
                }


                // var expenseResult = expenses.ToList().Select(exp => _exepenseGroupFactory.CreateDataShapedObject(exp,listOfFields));
                //above code is not working hence did the easy to return requested fields in the result.
                var expenseResult = expenses.ToList();

                List<object> dynamic = new List<object>();
                foreach (var e in expenseResult)
                {
                    dynamic.Add(_exepenseGroupFactory.CreateDataShapedObject(e, listOfFields));
                }


                //  var output = mapper.Map<IEnumerable<Dto.Expense>>(dynamic as D);
                return Ok(dynamic);
                //else
                //{
                //    var expensesForGroup = _repository.GetExpenses((int)expenseGroupId);
                //    if (expensesForGroup != null)
                //    {
                //        expense = expensesForGroup.FirstOrDefault(eg => eg.Id == id);
                //    }
                //}

                //if (expense != null)
                //{
                //    var returnValue = mapper.Map<Dto.Expense>(expense);
                //    return Ok(returnValue);
                //}
                //else
                //{
                //    return NotFound();
                //}

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
