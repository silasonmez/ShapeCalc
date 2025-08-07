using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

// EF için:
using System.Data.Entity;
using InputApi.Data;   // <— ApplicationDbContext burada

namespace InputApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // EF’nin veritabanı oluşturma/attach/migration yapmasını kapat
            Database.SetInitializer<ApplicationDbContext>(null);

            // Round-robin selector'u başlat
            var selector = new InputApi.Services.RoundRobinEndpointSelector();

            Application["EndpointSelector"] = selector;


            // (Şablondan gelen kayıtlar kalsın, sorun değil)
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
