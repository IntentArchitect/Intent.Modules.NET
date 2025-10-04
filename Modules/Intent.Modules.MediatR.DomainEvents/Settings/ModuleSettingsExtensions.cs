using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.MediatR.DomainEvents.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DomainEventSettings GetDomainEventSettings(this IApplicationSettingsProvider settings)
        {
            return new DomainEventSettings(settings.GetGroup("f8a6fc09-1eba-4d70-a336-82546659d323"));
        }
    }

    public class DomainEventSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DomainEventSettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }

        public bool CreateImplicitDomainEventHandlers() => bool.TryParse(_groupSettings.GetSetting("59ce20f9-0fe5-4c1c-8016-de94bfe1d4a2")?.Value.ToPascalCase(), out var result) && result;
    }
}