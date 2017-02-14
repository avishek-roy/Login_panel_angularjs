using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BOE.Startup))]
namespace BOE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
