using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DomainEventSettings GetDomainEventSettings(this IApplicationSettingsProvider settings)
        {
            return new DomainEventSettings(settings.GetGroup("f4b28e5f-12f3-409e-b6df-e1d6963c25fe"));
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

        public bool CreateImplicitDomainEventHandlers() => bool.TryParse(_groupSettings.GetSetting("5f32789b-dd8d-4c23-a0d5-0e090e7d1b4a")?.Value.ToPascalCase(), out var result) && result;
    }
}