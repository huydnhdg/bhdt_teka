using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TEKA.Startup))]
namespace TEKA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
