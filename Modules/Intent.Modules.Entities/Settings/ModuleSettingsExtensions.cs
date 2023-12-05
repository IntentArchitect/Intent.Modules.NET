using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.Settings
{

    public static class DomainSettingsExtensions
    {

        public static bool CreateEntityInterfaces(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("0456dafe-a46e-466b-bf23-1fb35c094899")?.Value.ToPascalCase(), out var result) && result;

        public static bool UseImplicitTagModeForEntities(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("9bb8d5fb-414d-4679-8bb3-5c91e1502720")?.Value.ToPascalCase(), out var result) && result;

        public static bool EnsurePrivatePropertySetters(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("0cf704e1-9a61-499a-bb91-b20717e334f5")?.Value.ToPascalCase(), out var result) && result;

        public static bool SeparateStateFromBehaviour(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("7692b7ce-4eb7-4245-ade9-d6b5fb684d80")?.Value.ToPascalCase(), out var result) && result;
    }
}