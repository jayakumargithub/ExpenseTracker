using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using ExpenseTracker.Repository;
using ExpenseTracker.Repository.Entities;

namespace ExpenseTracker.Api.Infrastructure
{
    // TODO: Add the following to SomeType's definition to see this visualizer when debugging instances of SomeType:
    // 
    //  [DebuggerVisualizer(typeof(RepositoriesInstaller))]
    //  [Serializable]
    //  public class SomeType
    //  {
    //   ...
    //  }
    // 
    /// <summary>
    /// A Visualizer for SomeType.  
    /// </summary>
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
