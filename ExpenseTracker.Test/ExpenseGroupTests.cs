using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using NUnit.Framework;
using Moq;
using ExpenseTracker.Api.Controllers;
using ExpenseTracker.Dto;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Test
{
    [TestFixture]
    public class ExpenseGroupTests
    {
        private List<Repository.Entities.ExpenseGroup> _expenseGroup;
        private List<Repository.Entities.ExpenseGroup> _noExpenseGroup;
        private IQueryable<Repository.Entities.ExpenseGroup> _expenseGroupQuery;
        private IQueryable<Repository.Entities.ExpenseGroup> _expenseGroupQueryNull;
        private IQueryable<Repository.Entities.ExpenseGroup> _noExpenseGroupQuery;
        private Repository.Entities.ExpenseGroup _exGroup;
        [OneTimeSetUp]
        public void Setup()
        {
            _expenseGroup = new List<Repository.Entities.ExpenseGroup>();
            _expenseGroup.Add(new Repository.Entities.ExpenseGroup { Description = "Test Description 1", Expenses = null, Id = 1, Title = "Test Title 1", UserId = "12" });
            _expenseGroup.Add(new Repository.Entities.ExpenseGroup { Description = "Test Description 2", Expenses = null, Id = 2, Title = "Test Title 2", UserId = "13" });
            _expenseGroupQuery = _expenseGroup.AsQueryable();
            _noExpenseGroup = new List<Repository.Entities.ExpenseGroup>();
            _noExpenseGroupQuery = _noExpenseGroup.AsQueryable();
            _exGroup = new Repository.Entities.ExpenseGroup();
            _expenseGroupQueryNull = null;

        }
        [Test]
        public void ExpenseGroupController_get_will_return_passed_all_expense_group_as_OkNegotiatedContentResult()
        {
            //arrange
            var mockRepo = new Mock<IExpenseTrackerRepository>();
            mockRepo.Setup(x => x.GetExpenseGroups()).Returns(_expenseGroupQuery);
            var controller = new ExpenseGroupController(mockRepo.Object); 
             
            //act
            var output = controller.Get(); 
            var response = output as OkNegotiatedContentResult<List<Dto.ExpenseGroup>>;

            //assert
            Assert.IsNotNull(response); 
            Assert.AreEqual(_expenseGroup.Count, response.Content.Count());


        }

        [Test]
        public void ExpenseGroupController_will_InternalServerErrorResult_exception_when_query_is_null()
        {
            var mockRepo = new Mock<IExpenseTrackerRepository>();

            mockRepo.Setup(x => x.GetExpenseGroups()).Returns(_expenseGroupQueryNull);
            var controller = new ExpenseGroupController(mockRepo.Object); 

            //assert
            var expected = controller.Get();

            //act
            Assert.IsInstanceOf<InternalServerErrorResult>(expected);
        }

        [Test]
        public void ExpenseGroupController_will_throw_NotFoundResult_when_no_data_Found()
        {
            //arrange
            var mockRepo = new Mock<IExpenseTrackerRepository>();
            mockRepo.Setup(x => x.GetExpenseGroups()).Returns(_noExpenseGroupQuery);
            var controller = new ExpenseGroupController(mockRepo.Object);

            //assert
            var expected = controller.Get();

            //act
            Assert.IsInstanceOf<NotFoundResult>(expected);
        }

        [Test]
        public void ExpenseGroupController_will_return_just_one_item_for_matching_id()
        {
            var mockRepo = new Mock<IExpenseTrackerRepository>();
            int expenseGroupId = 1;
            mockRepo.Setup(x => x.GetExpenseGroup(It.IsAny<int>())).Returns(_expenseGroupQuery.Single(x => x.Id == expenseGroupId));
            var controller = new ExpenseGroupController(mockRepo.Object);

            //assert
            var expected = controller.Get(expenseGroupId);

            var response = expected as OkNegotiatedContentResult<Dto.ExpenseGroup>;

            Assert.AreEqual(expenseGroupId, response.Content.Id);

        }
        

        [Test]
        public void ExpenseGroupController_will_throw_NotFoundResult_when_no_data_Found_in_Get_with_Id()
        {
            //arrange
            var mockRepo = new Mock<IExpenseTrackerRepository>();
            mockRepo.Setup(x => x.GetExpenseGroup(It.IsAny<int>())).Returns(_exGroup);
            var controller = new ExpenseGroupController(mockRepo.Object);

            //assert
            var expected = controller.Get();

            //act
            Assert.IsInstanceOf<NotFoundResult>(expected);
        }
    }
}
