using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Hangfire.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static HangfireSettings GetHangfireSettings(this IApplicationSettingsProvider settings)
        {
            return new HangfireSettings(settings.GetGroup("51fa8230-f836-453f-bdbf-1aef2e3a41cb"));
        }
    }

    public class HangfireSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public HangfireSettings(IGroupSettings groupSettings)
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
        public StorageOptions Storage() => new StorageOptions(_groupSettings.GetSetting("66b854a5-3a68-40bf-a283-8eff57f29a35")?.Value);

        public class StorageOptions
        {
            public readonly string Value;

            public StorageOptions(string value)
            {
                Value = value;
            }

            public StorageOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "None" => StorageOptionsEnum.None,
                    "InMemory" => StorageOptionsEnum.InMemory,
                    "SQLServer" => StorageOptionsEnum.SQLServer,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "None";
            }

            public bool IsInMemory()
            {
                return Value == "InMemory";
            }

            public bool IsSQLServer()
            {
                return Value == "SQLServer";
            }
        }

        public enum StorageOptionsEnum
        {
            None,
            InMemory,
            SQLServer,
        }

        public bool ShowDashboard() => bool.TryParse(_groupSettings.GetSetting("0f98d51a-8b18-477d-995f-677b3dd7d5b4")?.Value.ToPascalCase(), out var result) && result;

        public string DashboardURL() => _groupSettings.GetSetting("929eb3fd-8fd5-4852-9029-a5b7c45dd3a4")?.Value;

        public string DashboardTitle() => _groupSettings.GetSetting("e19d4511-ee3a-4f5e-8e78-16a1ac9e6dc6")?.Value;

        public bool ReadOnlyDashboard() => bool.TryParse(_groupSettings.GetSetting("c5ccd7bb-381c-4a63-86cb-827f11a0654c")?.Value.ToPascalCase(), out var result) && result;

        public bool ConfigureAsHangfireServer() => bool.TryParse(_groupSettings.GetSetting("bbffbc72-af03-4848-8842-dd061f1a00dd")?.Value.ToPascalCase(), out var result) && result;

        public string WorkerCount() => _groupSettings.GetSetting("e1b8de14-a59e-4323-ad77-259397d2c4e0")?.Value;

        public string JobRetentionHours() => _groupSettings.GetSetting("792db502-f103-47ab-af9b-066183720124")?.Value;
    }
}