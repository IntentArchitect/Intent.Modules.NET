using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.EntityFrameworkCore.BulkOperations
{
    internal static class DBSettingsExtensions
    {
        public static IGroupSettings GetDatabaseSettings(this IApplicationSettingsProvider settings)
        {
            return settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9");
        }
        public static bool AddSynchronousMethodsToRepositories(this IGroupSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("ce8b3b8e-fe64-4017-aa16-f56e768fc52d")?.Value.ToPascalCase(), out var result) && result;

    }
}
