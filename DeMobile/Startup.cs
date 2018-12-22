using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Cors;


[assembly: OwinStartup(typeof(DeMobile.Startup))]
namespace DeMobile
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}