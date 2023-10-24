using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static SwaggerSettings GetSwaggerSettings(this IApplicationSettingsProvider settings)
        {
            return new SwaggerSettings(settings.GetGroup("7182f792-547f-46f3-b178-a92ceefeb4f8"));
        }
    }

    public class SwaggerSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public SwaggerSettings(IGroupSettings groupSettings)
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

        public bool MarkNonNullableFieldsAsRequired() => bool.TryParse(_groupSettings.GetSetting("7faaa6d0-c739-430f-a953-c78fb5e0a302")?.Value.ToPascalCase(), out var result) && result;

        public bool UseSimpleSchemaIdentifiers() => bool.TryParse(_groupSettings.GetSetting("68427321-6418-474b-a816-26b406d7d262")?.Value.ToPascalCase(), out var result) && result;
    }
}