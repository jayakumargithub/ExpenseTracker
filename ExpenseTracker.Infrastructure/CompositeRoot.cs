using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Infrastructure
{
    public class CompositeRoot
    {
        public static IWindsorContainer Container;
        //public CompositeRoot()
        //{

        //    IRegistration[] components = {
        //        Component.For<IExpenseTrackerRepository>().ImplementedBy<ExpenseTrackerEFRepository>().LifestylePerWebRequest(),
        //        Component.For<ExpenseTrackerContext>().IsDefault()
        //    };
        //    Container.Register(components);

        //    return new ExpenseTrackerEFRepository
        //}
    }
}
