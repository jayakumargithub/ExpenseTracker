using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ExpenseTracker.Dto;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseMasterDataFactory
    {
        public ExpenseMasterDataFactory()
        {

        }



        public ExpenseGroupStatus CreateExpenseGroupStatus(Dto.ExpenseGroupStatus expenseGroupStatus)
        {
            return new ExpenseGroupStatus()
            {
                Description = expenseGroupStatus.Description,
                Id = expenseGroupStatus.Id
            };
        }


        public Dto.ExpenseGroupStatus CreateExpenseGroupStatus(ExpenseGroupStatus expenseGroupStatus)
        {
            return new Dto.ExpenseGroupStatus()
            {
                Description = expenseGroupStatus.Description,
                Id = expenseGroupStatus.Id
            };
        }
    }
}
