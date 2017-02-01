using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdvanceASPNET.Startup))]
namespace AdvanceASPNET
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
