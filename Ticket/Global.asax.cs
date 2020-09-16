using System.Data.Entity.Migrations;
using System.Globalization;
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
            // Formatting numbers, dates, etc.
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");

            // UI strings that we have localized.
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("tr-TR");
            var configuration = new Migrations.Configuration()
            {
                AutomaticMigrationDataLossAllowed = true
            };
            var migrator = new DbMigrator(configuration);
            migrator.Update();
        }
    }
}