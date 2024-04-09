using System;
using System.Linq;
using System.Text.Json;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Logging.Serilog.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static SerilogSettings GetSerilogSettings(this IApplicationSettingsProvider settings)
        {
            return new SerilogSettings(settings.GetGroup("627ffe24-50f0-4e12-8276-073a255523b0"));
        }
    }

    public class SerilogSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public SerilogSettings(IGroupSettings groupSettings)
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
        public SinksOptions[] Sinks() => JsonSerializer.Deserialize<string[]>(_groupSettings.GetSetting("e1e76d8d-298a-4f47-af08-0b08fdcc95c1")?.Value ?? "[]")?.Select(x => new SinksOptions(x)).ToArray();

        public class SinksOptions
        {
            public readonly string Value;

            public SinksOptions(string value)
            {
                Value = value;
            }

            public SinksOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "console" => SinksOptionsEnum.Console,
                    "file" => SinksOptionsEnum.File,
                    "graylog" => SinksOptionsEnum.Graylog,
                    "application-insights" => SinksOptionsEnum.ApplicationInsights,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsConsole()
            {
                return Value == "console";
            }

            public bool IsFile()
            {
                return Value == "file";
            }

            public bool IsGraylog()
            {
                return Value == "graylog";
            }

            public bool IsApplicationInsights()
            {
                return Value == "application-insights";
            }
        }

        public enum SinksOptionsEnum
        {
            Console,
            File,
            Graylog,
            ApplicationInsights,
        }
    }
}