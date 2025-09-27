using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Settings
{

    public static class DomainEventSettingsExtensions
    {

        public static bool CreateImplicitDomainEventHandlers(this DomainEventSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("59ce20f9-0fe5-4c1c-8016-de94bfe1d4a2")?.Value.ToPascalCase(), out var result) && result;
    }
}