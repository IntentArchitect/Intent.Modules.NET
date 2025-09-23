using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation.Settings
{

    public static class BlazorSettingsExtensions
    {

        public static bool CreateModelDefinitionValidators(this Intent.Modules.Blazor.Settings.Blazor groupSettings) => bool.TryParse(groupSettings.GetSetting("ab557f4a-cd5b-4ae1-bbc2-ba0522503956")?.Value.ToPascalCase(), out var result) && result;
    }
}