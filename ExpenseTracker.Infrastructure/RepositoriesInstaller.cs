using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Infrastructure
{
     
    public class RepositoriesInstaller :  IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IExpenseTrackerRepository>()
                    .ImplementedBy<ExpenseTrackerEFRepository>()
                    .LifestylePerWebRequest(),
            Component.For<ExpenseTrackerContext>().IsDefault());
        }
    }
}
