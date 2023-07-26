using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.HealthChecks.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static HealthChecks GetHealthChecks(this IApplicationSettingsProvider settings)
        {
            return new HealthChecks(settings.GetGroup("842cff41-d028-438f-913e-85932ce0f6e8"));
        }
    }

    public class HealthChecks : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public HealthChecks(IGroupSettings groupSettings)
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
        public PublishEventsOptions PublishEvents() => new PublishEventsOptions(_groupSettings.GetSetting("1405e444-058b-423c-83a7-217b3da1f9d3")?.Value);

        public class PublishEventsOptions
        {
            public readonly string Value;

            public PublishEventsOptions(string value)
            {
                Value = value;
            }

            public PublishEventsOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => PublishEventsOptionsEnum.None,
                    "azure-application-insights" => PublishEventsOptionsEnum.AzureApplicationInsights,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsAzureApplicationInsights()
            {
                return Value == "azure-application-insights";
            }
        }

        public enum PublishEventsOptionsEnum
        {
            None,
            AzureApplicationInsights,
        }

        public bool HealthChecksUI() => bool.TryParse(_groupSettings.GetSetting("68cad1ef-8e6b-4a51-a9b7-ec6042224cc8")?.Value.ToPascalCase(), out var result) && result;
    }
}