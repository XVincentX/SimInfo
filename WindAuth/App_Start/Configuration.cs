using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;


namespace WindAuth.App_Start
{
    public class Configuration : DbConfiguration
    {
        public Configuration()
        {
            SetDatabaseInitializer<WindAuth.Data.DataContext>(new MigrateDatabaseToLatestVersion<WindAuth.Data.DataContext, WindAuth.Migrations.Configuration>());
        }
    }
}