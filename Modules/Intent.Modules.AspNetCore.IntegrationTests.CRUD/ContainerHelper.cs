using Intent.Modules.AspNetCore.IntegrationTesting.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTests.CRUD
{
    internal class ContainerHelper
    {
        private static HashSet<DatabaseSettingsExtensions.DatabaseProviderOptionsEnum> _supportedDBs = new() { DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer, DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql };
        public static bool RequireCosmosContainer(IntentTemplateBase template)
        {
            return template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.CosmosDB") ||
                template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.EntityFrameworkCore") && template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum() == DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos;
        }

        public static bool RequireRdbmsEFContainer(IntentTemplateBase template)
        {
            return template.ExecutionContext.InstalledModules.Any(p => p.ModuleId == "Intent.EntityFrameworkCore") &&
                _supportedDBs.Contains(template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum());
        }
    }
}
