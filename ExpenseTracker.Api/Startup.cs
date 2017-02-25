using System.Web.Http;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ExpenseTracker.Infrastructure; 
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ExpenseTracker.Api.Startup))]

namespace ExpenseTracker.Api
{


    public class Startup
    {
        private static IWindsorContainer _container;
        public void Configuration(IAppBuilder app)
        {
             
            var config = new HttpConfiguration();


            _container = new WindsorContainer();
            _container.Install(FromAssembly.This());
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            var dependencyResolver = new WindsorDependencyResolver(_container);
            config.DependencyResolver = dependencyResolver;
            config.EnableCors();



            app.UseWebApi(WebApiConfig.Register(config));

        }
         
    }

     
}
