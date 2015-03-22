using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WorkFlow.Startup))]
namespace WorkFlow
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
