using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static MediatorSettings GetMediatorSettings(this IApplicationSettingsProvider settings)
        {
            return new MediatorSettings(settings.GetGroup("ad217e62-f3f2-4ec8-8ccf-62272a3e31e4"));
        }
    }

    public class MediatorSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public MediatorSettings(IGroupSettings groupSettings)
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

        public bool EnableLoggingBehaviour() => bool.TryParse(_groupSettings.GetSetting("17fd2373-eaca-4109-846d-2f55123ad2c5")?.Value.ToPascalCase(), out var result) && result;

        public bool EnablePerformanceBehaviour() => bool.TryParse(_groupSettings.GetSetting("a6ffb57a-8f07-445a-affd-0b0a4341bd8d")?.Value.ToPascalCase(), out var result) && result;

        public bool EnableUnhandledExceptionBehaviour() => bool.TryParse(_groupSettings.GetSetting("e2428551-ac90-4ba3-9357-cd7fbb9da3f4")?.Value.ToPascalCase(), out var result) && result;
    }
}