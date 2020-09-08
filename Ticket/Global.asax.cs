using System.Data.Entity.Migrations;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ticket
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            var configuration = new Migrations.Configuration()
            {
                AutomaticMigrationDataLossAllowed = true
            };
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}