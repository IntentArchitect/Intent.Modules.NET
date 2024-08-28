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

        public bool CaptureTraces() => bool.TryParse(_groupSettings.GetSetting("93dbe976-bebb-47e9-a59e-0367271f3dbb")?.Value.ToPascalCase(), out var result) && result;

        public bool CaptureLogs() => bool.TryParse(_groupSettings.GetSetting("7cbb4a05-7dac-4b06-b2da-635ab82dd125")?.Value.ToPascalCase(), out var result) && result;

        public bool ASPNETCoreInstrumentation() => bool.TryParse(_groupSettings.GetSetting("11352624-4a31-4ec9-8768-0dfef81cf3b1")?.Value.ToPascalCase(), out var result) && result;

        public bool CaptureMetrics() => bool.TryParse(_groupSettings.GetSetting("b919cb80-15dc-4f1e-820c-faaddf4c0063")?.Value.ToPascalCase(), out var result) && result;

        public bool HTTPInstrumentation() => bool.TryParse(_groupSettings.GetSetting("d41bdd38-bfc0-4c96-8cb2-0bd4b6f01f01")?.Value.ToPascalCase(), out var result) && result;

        public bool SQLInstrumentation() => bool.TryParse(_groupSettings.GetSetting("14d18546-83cd-46b6-8d0e-c9d098ece8e0")?.Value.ToPascalCase(), out var result) && result;

        public bool ProcessInstrumentation() => bool.TryParse(_groupSettings.GetSetting("dd698842-3b42-4ddd-8a90-cb67a12ed375")?.Value.ToPascalCase(), out var result) && result;

        public bool NETRuntimeInstrumentation() => bool.TryParse(_groupSettings.GetSetting("93367ae8-466f-4e0e-a84c-19eae4b80daa")?.Value.ToPascalCase(), out var result) && result;
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
                    "azure-monitor-opentelemetry-distro" => ExportOptionsEnum.AzureMonitorOpentelemetryDistro,
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

            public bool IsAzureMonitorOpentelemetryDistro()
            {
                return Value == "azure-monitor-opentelemetry-distro";
            }
        }

        public enum ExportOptionsEnum
        {
            Console,
            OpenTelemetryProtocol,
            AzureApplicationInsights,
            AzureMonitorOpentelemetryDistro,
        }
    }
}