using EFCache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Web;


namespace WindAuth.App_Start
{
    public class Configuration : DbConfiguration
    {
        public Configuration()
        {
            SetDatabaseInitializer<WindAuth.Data.DataContext>(new MigrateDatabaseToLatestVersion<WindAuth.Data.DataContext, WindAuth.Migrations.Configuration>());

            var transactionHandler = new CacheTransactionHandler(new InMemoryCache());

            AddInterceptor(transactionHandler);

            var cachingPolicy = new CachingPolicy();

            Loaded +=
              (sender, args) => args.ReplaceService<DbProviderServices>(
                (s, _) => new CachingProviderServices(s, transactionHandler,
                  cachingPolicy));

        }
    }
}