using Intent.Modules.AspNetCore.IntegrationTesting.Templates.MongoDbContainerFixture;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates
{
    internal class ContainerHelper
    {
        /// <summary>
        /// Database providers for which this module can generate an EF database fixture. Note that not all
        /// of these are container-backed — SQLite runs in-process — but they all resolve to the same
        /// <c>EFContainerFixture</c> and the same wiring in <c>IntegrationTestWebAppFactory</c>.
        /// </summary>
        private static HashSet<DatabaseSettingsExtensions.DatabaseProviderOptionsEnum> _supportedEFDBs = new()
        {
            DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer,
            DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql,
            DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SQLLite,
        };
        public static bool RequireCosmosContainer(IntentTemplateBase template)
        {
            return template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.CosmosDB") ||
                (template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.EntityFrameworkCore") && template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum() == DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos);
        }

        public static bool RequireRdbmsEFContainer(IntentTemplateBase template)
        {
            return template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.EntityFrameworkCore") &&
                _supportedEFDBs.Contains(template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum());
        }

        internal static bool RequireMongoContainer(IntentTemplateBase template)
        {
            return template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.MongoDb");
        }

        internal static bool RequireMongoFrameworkContainer(IntentTemplateBase template)
        {
            return template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.MongoDb.MongoFramework");
        }
    }
}
