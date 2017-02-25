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
    public class ExpenseGroupController : ApiController
    {
        private readonly IExpenseTrackerRepository _repository;
        private readonly IMapper _mapper;
        private const int MaxPageSize = 10;
        private ExepenseGroupFactory _exepenseGroupFactory;
        public ExpenseGroupController(IExpenseTrackerRepository repository)
        {
            _repository = repository;
            _mapper = Mapping.Configuration();
            _exepenseGroupFactory = new ExepenseGroupFactory();
        }

        //for sorting support install-package system.linq.dynamic
        /*
         descing order url Use minus(-) for the property http://localhost:51825/api/expensegroup?sort=-expenseGroupStatusId,title
         * http://localhost:51825/api/expensegroup/?page=1&pagesize=3&sort=title&status=open
         */
        [Route("expensegroup", Name="ExpenseGroupList")]
       [HttpGet]
        public IHttpActionResult Get(string fields=null,string sort = "id", string status = null, string userId = null, int page = 1, int pageSize = 5) 
        {
            try
            
            {

                bool includeExpenses = false;
                List<string> listofFields = new List<string>();
                if (!string.IsNullOrEmpty(fields))
                {
                    listofFields = fields.ToLower().Split(',').ToList();
                    includeExpenses = listofFields.Any(f => f.Contains("expense"));
                }


                int statusId = -1;
                if (status != null)
                {
                    switch (status.ToLower())
                    {
                        case "open" :
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

                    expenseGroups = _repository.GetExpenseGroups()
                        .ApplySortMultiColumns(sort) //sorting
                        .Where(eg => (statusId == -1|| eg.ExpenseGroupStatusId == statusId)) // filtering
                        .Where(eg => (userId == null || eg.UserId == userId)); //filtering

                if (pageSize > MaxPageSize)
                {
                    pageSize = MaxPageSize;
                }

                var totalCount = expenseGroups.Count();
                var totalPages = (int) Math.Ceiling((double) totalCount / pageSize);


                var urlHelper = new UrlHelper(Request);
                var prevLink = page > 1
                    ? urlHelper.Link("ExpenseGroupList",
                        new
                        {
                            page = page - 1,
                            pageSize = pageSize,
                            sort = sort,
                            fields = fields,
                            status = status,
                            userId = userId
                        })
                    : "";
                var nextLink = page < totalPages
                    ? urlHelper.Link("ExpenseGroupList",
                        new
                        {
                            page = page + 1,
                            pageSize = pageSize,
                            sort = sort,
                            fields = fields,
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

                HttpContext.Current.Response.Headers.Add("X-Pagination",
                    Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

                var resultQuery = expenseGroups.Skip(pageSize * (page - 1)).Take(pageSize);
                var expDtoEntity = _mapper.Map<IEnumerable<Dto.ExpenseGroup>>(resultQuery);
               // var output = _exepenseGroupFactory.CreateDataShapeObject(expDtoEntity,listofFields);
                //if (output != null)
                //{
                //    return Ok(output);
                //}
                //else
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

        [HttpPost]
        public IHttpActionResult Post([FromBody] Dto.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }

                var expGroup = _mapper.Map<Repository.Entities.ExpenseGroup>(expenseGroup);
                var result = _repository.InsertExpenseGroup(expGroup);
                if (result.Status == RepositoryActionStatus.Created)
                {
                    var newExpGroup = _mapper.Map<Dto.ExpenseGroup>(result.Entity);
                    return Created(Request.RequestUri + "/" + newExpGroup.Id.ToString(), newExpGroup);
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpPut]
        public IHttpActionResult Put(int id, [FromBody] Dto.ExpenseGroup expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }

                var expGroup = _mapper.Map<Repository.Entities.ExpenseGroup>(expenseGroup);
                var result = _repository.UpdateExpenseGroup(expGroup);
                if (result.Status == RepositoryActionStatus.Updated)
                {
                    var updatedExpGroup = _mapper.Map<Dto.ExpenseGroup>(result.Entity);
                    return Ok(updatedExpGroup);
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

        //use JsonPach -> Install-Package Marvin.JsonPatch
        [HttpPatch]
        public IHttpActionResult Patch(int id, [FromBody] JsonPatchDocument<Dto.ExpenseGroup> expenseGroup)
        {
            try
            {
                if (expenseGroup == null)
                {
                    return BadRequest();
                }

                var expenseGroupEntity = _repository.GetExpenseGroup(id);
                if (expenseGroupEntity == null)
                {
                    return NotFound();
                }
                //Matp to DTO
                var eg = _mapper.Map<Dto.ExpenseGroup>(expenseGroupEntity);
                //apply changes to DTO
                expenseGroup.ApplyTo(eg);

                var egEntity = _mapper.Map<Repository.Entities.ExpenseGroup>(eg);


                var result = _repository.UpdateExpenseGroup(egEntity);

                if (result.Status == RepositoryActionStatus.Updated)
                {
                    var patchedExpenseGroup = _mapper.Map<Dto.ExpenseGroup>(result.Entity);
                    return Ok(patchedExpenseGroup);

                }

                return BadRequest();


            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }

        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _repository.DeleteExpenseGroup(id);
                if (result.Status == RepositoryActionStatus.Deleted)
                {
                    return StatusCode(HttpStatusCode.NoContent);
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
