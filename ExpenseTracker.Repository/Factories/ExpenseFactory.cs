using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;
using System.Reflection;
using ExpenseTracker.Repository.Entities;
namespace ExpenseTracker.Repository.Factories
{
    public class ExpenseFactory
    {

        public ExpenseFactory()
        {

        }

        public Dto.Expense CreateExpense(Expense expense)
        {
            return new Dto.Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }



        public Expense CreateExpense(Dto.Expense expense)
        {
            return new Expense()
            {
                Amount = expense.Amount,
                Date = expense.Date,
                Description = expense.Description,
                ExpenseGroupId = expense.ExpenseGroupId,
                Id = expense.Id
            };
        }


        public object CreateDataShapedObject(Expense expense, List<string> lstOfFields)
        {

            return CreateDataShapedObject(CreateExpense(expense), lstOfFields);
        }


        public object CreateDataShapedObject(Dto.Expense expense, List<string> lstOfFields)
        {

            if (!lstOfFields.Any())
            {
                return expense;
            }
            else
            {

                // create a new ExpandoObject & dynamically create the properties for this object

                ExpandoObject objectToReturn = new ExpandoObject();
                foreach (var field in lstOfFields)
                {
                    // need to include public and instance, b/c specifying a binding flag overwrites the
                    // already-existing binding flags.

                    var fieldValue = expense.GetType()
                        .GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance)
                        .GetValue(expense, null);

                    // add the field to the ExpandoObject
                    ((IDictionary<String, Object>)objectToReturn).Add(field, fieldValue);
                }

                return objectToReturn;
            }
        }





    }
}