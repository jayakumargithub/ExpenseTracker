using System.Web;
using System.Web.Http;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ExpenseTracker.Infrastructure;

namespace ExpenseTracker.Api
{
    public class WebApiApplication : HttpApplication
    {
        private IWindsorContainer _container;

        protected void Application_Start()
        {
            // ConfigureWindsor(GlobalConfiguration.Configuration);
            //GlobalConfiguration.Configure(c => WebApiConfig.Register());

            //AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            //FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            //RouteConfig.RegisterRoutes(RouteTable.Routes);
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public void ConfigureWindsor(HttpConfiguration config)
        {
            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            var dependcyResolver = new WindsorDependencyResolver(_container);
            config.DependencyResolver = dependcyResolver;
        }
    }
}