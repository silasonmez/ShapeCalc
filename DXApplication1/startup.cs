using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using DXApplication1.Models; // kendi namespace’ini kullan

[assembly: OwinStartup(typeof(DXApplication1.Startup))]
namespace DXApplication1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // BU 3 SATIR OLMAZSA SignInManager NULL olur!
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
        


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });



        }
    }
}
