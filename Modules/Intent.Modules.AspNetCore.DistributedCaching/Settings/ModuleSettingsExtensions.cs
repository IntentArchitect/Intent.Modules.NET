using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.DistributedCaching.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DistributedCachingSettings GetDistributedCachingSettings(this IApplicationSettingsProvider settings)
        {
            return new DistributedCachingSettings(settings.GetGroup("49ad40aa-eeb7-4d44-9b20-947a784dc12d"));
        }
    }

    public class DistributedCachingSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DistributedCachingSettings(IGroupSettings groupSettings)
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

        public ProviderOptions Provider() => new ProviderOptions(_groupSettings.GetSetting("82233474-2fba-4033-b236-97d5cc4d3e04")?.Value);

        public class ProviderOptions
        {
            public readonly string Value;

            public ProviderOptions(string value)
            {
                Value = value;
            }

            public ProviderOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "memory" => ProviderOptionsEnum.Memory,
                    "stack-exchange-redis" => ProviderOptionsEnum.StackExchangeRedis,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsMemory()
            {
                return Value == "memory";
            }

            public bool IsStackExchangeRedis()
            {
                return Value == "stack-exchange-redis";
            }
        }

        public enum ProviderOptionsEnum
        {
            Memory,
            StackExchangeRedis,
        }
    }
}