using System.Net.Http.Headers;
using System.Web.Http;
using Castle.Windsor; 
using Newtonsoft.Json.Serialization;

namespace ExpenseTracker.Api
{
    public static class WebApiConfig
    {
        
        public static HttpConfiguration Register(HttpConfiguration config)
        {
            //var config = new HttpConfiguration();
           // ConfigureWindsor(GlobalConfiguration.Configuration);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
               name: "DefaultRouting",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { id = RouteParameter.Optional }
           );

            //suport Json format
           // config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));
            //stop supporting xml formatt
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            //format proper josn style 
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //format with camel casing
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));

           //config.MessageHandlers.Add(new CacheCow.Server.CachingHandler(config));
            return config;
        }

 
    }
}
