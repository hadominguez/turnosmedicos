using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Turnos_Medicos.Startup))]
namespace Turnos_Medicos
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
