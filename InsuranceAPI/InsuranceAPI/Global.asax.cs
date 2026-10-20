using System.Web.Http;

namespace InsuranceAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            // SwaggerConfig.Register() — НЕ ВЫЗЫВАТЬ!
            // Он регистрируется автоматически через [assembly: PreApplicationStartMethod]
        }
    }
}