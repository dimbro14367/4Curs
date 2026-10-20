using System.Web.Http;
using System.Web.Http.Cors;

namespace InsuranceAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // CORS — чтобы можно было дергать API из Swagger / мобильного приложения
            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            // Маршрутизация по атрибутам
            config.MapHttpAttributeRoutes();

            // Резервный маршрут
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Отдаём JSON вместо XML
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            var jsonFormatter = config.Formatters.JsonFormatter;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}