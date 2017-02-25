using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using Castle.Core.Internal;
using ExpenseTracker.Repository.Entities;
using System.Globalization;
using System.Linq.Dynamic;


namespace ExpenseTracker.Api.Extentions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplySortMultiColumns<T>(this IQueryable<T> source, string sort)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sort == null)
            {
                return source;
            }

            // split the sort string
            var lstSort = sort.Split(',');

            // run through the sorting options and apply them - in reverse
            // order, otherwise results will come out sorted by the last 
            // item in the string first!
            foreach (var sortOption in lstSort.Reverse())
            {
                // if the sort option starts with "-", we order
                // descending, ortherwise ascending

                if (sortOption.StartsWith("-"))
                {
                    source = source.OrderBy(sortOption.Remove(0, 1) + " descending");
                }
                else
                {
                    source = source.OrderBy(sortOption);
                }

            }

            return source;
        }
        


        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, string sortExpression)
        {
            if (source == null)
                throw new ArgumentNullException("source", "source is null.");

            if (string.IsNullOrEmpty(sortExpression))
                throw new ArgumentException("sortExpression is null or empty.", "sortExpression");

            var parts = sortExpression.Split(' ');
            var isDescending = false;
            var tType = typeof(T);

            if (parts.Length > 0 && parts[0] != "")
            {
                var propertyName = parts[0];

                if (parts.Length > 1)
                {
                    isDescending = parts[1].ToLower().Contains("esc");
                }

                var properties = tType.GetProperties();
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                PropertyInfo prop = tType.GetProperty(myTI.ToTitleCase(propertyName));
                
                   
                

                if (prop == null)
                {
                    throw new ArgumentException(string.Format("No property '{0}' on type '{1}'", propertyName, tType.Name));
                }

                var funcType = typeof(Func<,>)
                    .MakeGenericType(tType, prop.PropertyType);

                var lambdaBuilder = typeof(Expression)
                    .GetMethods()
                    .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2)
                    .MakeGenericMethod(funcType);

                var parameter = Expression.Parameter(tType);
                var propExpress = Expression.Property(parameter, prop);

                var sortLambda = lambdaBuilder
                    .Invoke(null, new object[] { propExpress, new ParameterExpression[] { parameter } });

                MethodInfo first = null;
                foreach (var x in typeof(Queryable).GetMethods())
                {
                    if (x.Name == (isDescending ? "OrderByDescending" : "OrderBy") && x.GetParameters().Length == 2)
                    {
                        first = x;
                        break;
                    }
                }
                if (first != null)
                {
                    var sorter = first
                        .MakeGenericMethod(new[] { tType, prop.PropertyType });

                    return (IQueryable<T>)sorter
                        .Invoke(null, new object[] { source, sortLambda });
                }
            }

            return source;
        }
    }
}