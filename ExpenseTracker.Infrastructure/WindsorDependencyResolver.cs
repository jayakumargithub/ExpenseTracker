using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using Castle.Windsor;

namespace ExpenseTracker.Infrastructure
{
    public class WindsorDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver
    {
        private readonly IWindsorContainer _container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            _container = container;
        }

        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            return _container.Kernel.HasComponent(serviceType) ? _container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (!_container.Kernel.HasComponent(serviceType))
            {
                return new object[0];
            }
            return _container.ResolveAll(serviceType).Cast<object>();
        }

        public IDependencyScope BeginScope()
        {
            return new WindsorDependencyScope(_container);
        }
    }
}