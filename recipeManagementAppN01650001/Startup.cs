using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(recipeManagementAppN01650001.Startup))]
namespace recipeManagementAppN01650001
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
