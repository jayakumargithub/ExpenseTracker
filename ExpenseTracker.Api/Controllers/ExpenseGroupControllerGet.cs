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
 

namespace ExpenseTracker.Api.Controllers
{
    [RoutePrefix("api")]
    public class ExpenseGroupGetController : ApiController
    {
        private readonly IExpenseTrackerRepository _repository;
        private readonly IMapper _mapper;
        private const int MaxPageSize = 10;
        private ExepenseGroupFactory _exepenseGroupFactory;
        public ExpenseGroupGetController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
            _mapper = Mapping.Configuration();
            _exepenseGroupFactory = new ExepenseGroupFactory();
        }

        //http://localhost:51825/api/expensegroupget

        [Route("expensegroupget", Name = "ExpenseGroupGet")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            try

            { 
                var expenseGroups = _repository.GetExpenseGroups(); 
                var expDtoEntity = _mapper.Map<IEnumerable<Dto.ExpenseGroup>>(expenseGroups.ToList());
                if (expDtoEntity != null)
                {
                    return Ok(expDtoEntity);
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
        //same as above method, just implemented sorting as well
        //Sorting Ascending, http://localhost:51825/api/expensegroupgetsort/?sort=expensegroupstatusid,title
       // Sorting Decesending put - before the expression) http://localhost:51825/api/expensegroupgetsort/?sort=-expensegroupstatusid,title
        [Route("expensegroupgetsort", Name = "ExpenseGroupGetSort")]
        [HttpGet]
        public IHttpActionResult Get(string sort="id")
        {
            try
            {

                var expenseGroups = _repository.GetExpenseGroups()
                     .ApplySortMultiColumns(sort); //sorting
                var expDtoEntity = _mapper.Map<IEnumerable<Dto.ExpenseGroup>>(expenseGroups.ToList());
                if (expDtoEntity != null)
                {
                    return Ok(expDtoEntity);
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

        //http://localhost:51825/api/expensegroupget/2
        [HttpGet]

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


        //Ascending , filter by Status http://localhost:51825/api/expensegroupgetfilter/?status=open&sort=description filter and direction
        //Descending http://localhost:51825/api/expensegroupgetfilter/?status=open&-sort=description - sorting
       // http://myexpensetrackerapi2017.azurewebsites.net/api/expensesgroup/?status=open - filter
        [Route("expensegroupgetfilter", Name = "ExpenseGroupGetFilter")]
        [HttpGet]
        public IHttpActionResult Get(string sort="id",string status=null,string userId = null)
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


                var expenseGroup = _repository.GetExpenseGroups()
                  .ApplySortMultiColumns(sort) //sorting
                  .Where(eg => (statusId == -1 || eg.ExpenseGroupStatusId == statusId)) // filtering
                  .Where(eg => (userId == null || eg.UserId == userId)).ToList(); //filtering

                var expenseGroupMaped = _mapper.Map<IEnumerable<Repository.Entities.ExpenseGroup>, IEnumerable<Dto.ExpenseGroup>>(expenseGroup);


                if (expenseGroupMaped == null)
                {
                    return NotFound();
                }
                 
                return Ok(expenseGroupMaped);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }

        [Route("expensegroupgetpaging", Name = "ExpenseGroupGetPaging")]
        [HttpGet]
        public IHttpActionResult Get(string sort = "id", string status = null, string userId = null,int page=1,int pageSize=5)
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


                if(pageSize > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }

                var expenseGroup = _repository.GetExpenseGroups()
                  .ApplySortMultiColumns(sort) //sorting
                  .Where(eg => (statusId == -1 || eg.ExpenseGroupStatusId == statusId)) // filtering
                  .Where(eg => (userId == null || eg.UserId == userId)); //filtering

                var totalCount = expenseGroup.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


                var urlHelper = new UrlHelper(Request);
                var prevLink = page > 1
                    ? urlHelper.Link("ExpenseGroupGetPaging",
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
                    ? urlHelper.Link("ExpenseGroupGetPaging",
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
                var output = expenseGroup.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                 
                var expenseGroupMaped = _mapper.Map<IEnumerable<Repository.Entities.ExpenseGroup>, IEnumerable<Dto.ExpenseGroup>>(output);


                if (expenseGroupMaped == null)
                {
                    return NotFound();
                }

                return Ok(expenseGroupMaped);
            }
            catch (Exception )
            {

                return InternalServerError();
            }
        }

        // http://localhost:51825/api/expensegroupgetwithexpenses?fields=id,title,expenses.id,expenses.amount
        //http://myexpensetrackerapi2017.azurewebsites.net/api/expensegroupgetwithexpenses?fields=id,title,expenses
        [Route("expensegroupgetwithexpenses", Name = "ExpenseGroupGetPagingWithExpenses")]
        [HttpGet]
        public IHttpActionResult Get(string sort = "id", string status = null, string userId = null,string fields=null, int page = 1, int pageSize = 5)
        {
            try
            {
                bool includeExpenses = false;
                List<string> listofFields = new List<string>();
                if (!string.IsNullOrEmpty(fields))
                {
                    listofFields = fields.ToLower().Split(',').ToList();
                    includeExpenses = listofFields.Any(f => f.Contains("expenses"));
                }


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

                IQueryable<Repository.Entities.ExpenseGroup> expenseGroups = null;
                if (includeExpenses)
                {
                    expenseGroups = _repository.GetExpenseGroupsWithExpenses();
                }
                else
                {
                    expenseGroups = _repository.GetExpenseGroups();
                }


                expenseGroups = expenseGroups
                  .ApplySortMultiColumns(sort) //sorting
                  .Where(eg => (statusId == -1 || eg.ExpenseGroupStatusId == statusId)) // filtering
                  .Where(eg => (userId == null || eg.UserId == userId)); //filtering

                if (pageSize > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }

                var totalCount = expenseGroups.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                 

                var urlHelper = new UrlHelper(Request);
                var prevLink = page > 1
                    ? urlHelper.Link("ExpenseGroupGetPaging",
                        new
                        {
                            sort = sort,
                            status = status,
                            userId = userId,
                            fields = fields,
                            page = page - 1,
                            pageSize = pageSize 
                        })
                    : "";
                var nextLink = page < totalPages
                    ? urlHelper.Link("ExpenseGroupGetPaging",
                        new
                        {

                            sort = sort,
                            status = status,
                            userId = userId,
                            fields = fields,
                            page = page + 1,
                            pageSize = pageSize 
                        })
                    : "";

                var paginationHeader = new
                {
                    currentpage = page,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = totalPages,
                    fields=fields,
                    previousPageLink = prevLink,
                    nextPageLink = nextLink 

                };

                HttpContext.Current.Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));
                var output = expenseGroups.Skip(pageSize * (page - 1)).Take(pageSize).ToList();
                var expenseGroupMaped = _mapper.Map<IEnumerable<Repository.Entities.ExpenseGroup>, IEnumerable<Dto.ExpenseGroup>>(output); 
                var expenseGroupOutput = expenseGroupMaped.Select(exp => _exepenseGroupFactory.CreateDataShapedObject(exp, listofFields));  

                if (expenseGroupMaped == null)
                {
                    return NotFound();
                }

                return Ok(expenseGroupOutput);
            }
            catch (Exception e)
            {
                Console.Write(e.StackTrace);
                return InternalServerError();
            }
        }


        [Route("expensegrouppost", Name = "ExpenseGroupPost")]
        [HttpPost]
        public IHttpActionResult Post([FromBody]Dto.ExpenseGroup expenseGroup)
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
                    return Created<Dto.ExpenseGroup>(Request.RequestUri + "/" + newExGp.Id.ToString(), newExGp);
                }

                return BadRequest();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [Route("expensegroupput", Name = "ExpenseGroupPut")]
        public IHttpActionResult Put(int id, [FromBody]Dto.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                    return BadRequest();

                // map
                var eg =_mapper.Map<Repository.Entities.ExpenseGroup>(expenseGroup);

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

    }
}
