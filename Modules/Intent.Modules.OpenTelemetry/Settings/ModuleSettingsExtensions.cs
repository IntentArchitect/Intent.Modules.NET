using System;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.OpenTelemetry.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static OpenTelemetry GetOpenTelemetry(this IApplicationSettingsProvider settings)
        {
            return new OpenTelemetry(settings.GetGroup("ea731c11-8d05-46e0-8040-7ee95a78e27e"));
        }
    }

    public class OpenTelemetry : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public OpenTelemetry(IGroupSettings groupSettings)
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

        public bool CaptureLogs() => bool.TryParse(_groupSettings.GetSetting("7cbb4a05-7dac-4b06-b2da-635ab82dd125")?.Value.ToPascalCase(), out var result) && result;

        public bool HTTPInstrumentation() => bool.TryParse(_groupSettings.GetSetting("d41bdd38-bfc0-4c96-8cb2-0bd4b6f01f01")?.Value.ToPascalCase(), out var result) && result;

        public bool SQLInstrumentation() => bool.TryParse(_groupSettings.GetSetting("14d18546-83cd-46b6-8d0e-c9d098ece8e0")?.Value.ToPascalCase(), out var result) && result;
        public ExportOptions Export() => new ExportOptions(_groupSettings.GetSetting("ccbdc18f-b1c3-4f4b-a1a7-fa29c626ce0e")?.Value);

        public class ExportOptions
        {
            public readonly string Value;

            public ExportOptions(string value)
            {
                Value = value;
            }

            public ExportOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "console" => ExportOptionsEnum.Console,
                    "open-telemetry-protocol" => ExportOptionsEnum.OpenTelemetryProtocol,
                    "azure-application-insights" => ExportOptionsEnum.AzureApplicationInsights,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsConsole()
            {
                return Value == "console";
            }

            public bool IsOpenTelemetryProtocol()
            {
                return Value == "open-telemetry-protocol";
            }

            public bool IsAzureApplicationInsights()
            {
                return Value == "azure-application-insights";
            }
        }

        public enum ExportOptionsEnum
        {
            Console,
            OpenTelemetryProtocol,
            AzureApplicationInsights,
        }
    }
}