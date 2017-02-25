using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using AutoMapper;
using ExpenseTracker.Repository;
using ExpenseTracker.Api.AutoMapper;
using Marvin.JsonPatch;
using ExpenseTracker.Api.Extentions;
using System.Web.Http.Cors;

namespace ExpenseTracker.Api.Controllers
{
    /// <summary>
    /// 
    /// 
    /// </summary>
    [RoutePrefix("api")]
    [EnableCors("*","*","*")]
    public class ExpensesGroupController : ApiController
    {

        private readonly IExpenseTrackerRepository _repository;
        private readonly IMapper _mapper;
        private const int MaxPageSize = 10;
        private ExepenseGroupFactory _exepenseGroupFactory;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public ExpensesGroupController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
            _mapper = Mapping.Configuration();
            _exepenseGroupFactory = new ExepenseGroupFactory();
        }

        /// <summary>
        /// 
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup -all expense group
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup?sort=title,description --sorting ascending
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup?sort=-title,description --sorting descending
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup/?status=open  --fitler
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup?page=2&pagesize=2
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup?page=2&pagesize=2&sort=description&status=open
        /// </summary>
        /// <returns></returns>
        //[Route("expensesgroup", Name = "ExpensesGroup")]
        [HttpGet]
        [Route("expensesgroup", Name = "ExpensesGroup")]
        public IHttpActionResult Get(string sort = "id", string status = null, string userId = null, int page = 1, int pageSize = 5)
        {
            try

            {
                int statusId = -1;
                if (status != null)
                {
                    switch (status.ToLower())
                    {
                        case "open":
                            statusId = 1;
                            break;
                        case "confirmed":
                            statusId = 2;
                            break;
                        case "processed":
                            statusId = 3;
                            break;
                        default:
                            break;
                    }
                }

                if (pageSize > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }

                var expenseGroups = _repository.GetExpenseGroups().ApplySortMultiColumns(sort)
                     .Where(eg => (statusId == -1 || eg.ExpenseGroupStatusId == statusId))
                  .Where(eg => (userId == null || eg.UserId == userId));


                var totalCount = expenseGroups.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


                var urlHelper = new UrlHelper(Request);
                var prevLink = page > 1
                    ? urlHelper.Link("ExpensesGroup",
                        new
                        {
                            page = page - 1,
                            pageSize = pageSize,
                            sort = sort,
                            status = status,
                            userId = userId
                        })
                    : "";
                var nextLink = page < totalPages
                    ? urlHelper.Link("ExpensesGroup",
                        new
                        {
                            page = page + 1,
                            pageSize = pageSize,
                            sort = sort,
                            status = status,
                            userId = userId
                        })
                    : "";

                var paginationHeader = new
                {
                    currentpage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink


                };

                HttpContext.Current.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));
                var output = expenseGroups.Skip(pageSize * (page - 1)).Take(pageSize).ToList();

                var expenseGroupMapped = _mapper.Map<IEnumerable<Repository.Entities.ExpenseGroup>, IEnumerable<Dto.ExpenseGroup>>(output);

                if (expenseGroupMapped != null)
                {
                    return Ok(expenseGroupMapped);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("error is :" + e.StackTrace);
                return InternalServerError();
            }
        }

        /// <summary>
        /// 
        /// http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup/1
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        
        public IHttpActionResult Get(int id)
        {
            try
            {
                var expenseGroup = _repository.GetExpenseGroup(id);
                if (expenseGroup == null)
                {
                    return NotFound();
                }


                var output = _mapper.Map<Dto.ExpenseGroup>(expenseGroup);
                return Ok(output);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expenseGroup"></param>
        /// <returns></returns> 
        [HttpPost]
        [Route("expensesgroup")]
        public IHttpActionResult Post(Dto.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }

                // try mapping & saving
                var ex = _mapper.Map<Repository.Entities.ExpenseGroup>(expenseGroup);


                var result = _repository.InsertExpenseGroup(ex);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    // map to dto

                    var newExGp = _mapper.Map<Dto.ExpenseGroup>(result.Entity);
                    return Created(Request.RequestUri + "/" + newExGp.Id.ToString(), newExGp);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="expenseGroup"></param>
        /// <returns></returns>
        [HttpPut] 
        public IHttpActionResult Put(int id, Dto.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                    return BadRequest();

                // map
                var eg = _mapper.Map<Repository.Entities.ExpenseGroup>(expenseGroup);

                var result = _repository.UpdateExpenseGroup(eg);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    // map to dto
                    var updatedExpenseGroup = _mapper.Map<Dto.ExpenseGroup>(result.Entity);
                    return Ok(updatedExpenseGroup);
                }
                else if (result.Status == RepositoryActionStatus.NotFound)
                {
                    return NotFound();
                }

                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _repository.DeleteExpenseGroup(id);
                if(result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
                }
                else {
                    return NotFound();
                }
                return BadRequest();
            }
            catch(Exception ex)
            {
                return InternalServerError();
            }
        }

        /// <summary>
        /// 
        /// Patch standard : https://tools.ietf.org/html/rfc6902
        /// nuget package : Marvin.Json.Patch
        /// Content-Type: applicatin/josn-patch+json
        /// Note: Need to add formater in webApiConfig
        /// </summary>
        /// <param name="id"></param>
        ///  /// <param name="expensePatchDoc"></param>
        /// <returns></returns>
        public IHttpActionResult Patch(int id,JsonPatchDocument<Dto.ExpenseGroup> expensePatchDoc)
        {
            try
            {
                if (expensePatchDoc == null)
                {
                    return BadRequest();
                }
                var expenseGroup = _repository.GetExpenseGroup(id);
                if(expenseGroup == null)
                {
                    return NotFound();
                }

                var eg = _mapper.Map<Dto.ExpenseGroup>(expenseGroup);
                expensePatchDoc.ApplyTo(eg);

                var itemToUpdate = _mapper.Map<Repository.Entities.ExpenseGroup>(eg);
                var updated = _repository.UpdateExpenseGroup(itemToUpdate);

                if(updated.Status == RepositoryActionStatus.Updated)
                {
                    var patchedExpensesGroup = _mapper.Map<Dto.ExpenseGroup>(updated.Entity);
                    return Ok(patchedExpensesGroup);
                }

                return BadRequest();


            }
            catch(Exception ex)
            {
                return InternalServerError();
            }
        }
    }
}
