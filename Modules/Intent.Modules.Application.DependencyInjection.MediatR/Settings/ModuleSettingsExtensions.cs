using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.MediatR.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static MediatRSettings GetMediatRSettings(this IApplicationSettingsProvider settings)
        {
            return new MediatRSettings(settings.GetGroup("ee01965a-8208-4fa2-b38e-2a3c95b5b67f"));
        }
    }

    public class MediatRSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public MediatRSettings(IGroupSettings groupSettings)
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

        public bool UsePreCommercialVersion() => bool.TryParse(_groupSettings.GetSetting("9a60a38a-fd0d-4272-bd0a-42559e5c065f")?.Value.ToPascalCase(), out var result) && result;
    }
}