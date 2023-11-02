using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.Settings
{

    public static class DatabaseSettingsExtensions
    {

        public static bool AddSynchronousMethodsToRepositories(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("ce8b3b8e-fe64-4017-aa16-f56e768fc52d")?.Value.ToPascalCase(), out var result) && result;
    }
}