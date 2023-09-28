using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.MultiTenancy.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static MultitenancySettings GetMultitenancySettings(this IApplicationSettingsProvider settings)
        {
            return new MultitenancySettings(settings.GetGroup("41ae5a02-3eb2-42a6-ade2-322b3c1f1115"));
        }
    }

    public class MultitenancySettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public MultitenancySettings(IGroupSettings groupSettings)
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

        public StrategyOptions Strategy() => new StrategyOptions(_groupSettings.GetSetting("e15fe0fb-be28-4cc5-8b85-37a07b7ca160")?.Value);

        public class StrategyOptions
        {
            public readonly string Value;

            public StrategyOptions(string value)
            {
                Value = value;
            }

            public StrategyOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "header" => StrategyOptionsEnum.Header,
                    "claim" => StrategyOptionsEnum.Claim,
                    "host" => StrategyOptionsEnum.Host,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsHeader()
            {
                return Value == "header";
            }

            public bool IsClaim()
            {
                return Value == "claim";
            }

            public bool IsHost()
            {
                return Value == "host";
            }
        }

        public enum StrategyOptionsEnum
        {
            Header,
            Claim,
            Host
        }

        public StoreOptions Store() => new StoreOptions(_groupSettings.GetSetting("e430d7bc-fac3-4528-93e8-e3f38bc0b925")?.Value);

        public class StoreOptions
        {
            public readonly string Value;

            public StoreOptions(string value)
            {
                Value = value;
            }

            public StoreOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "configuration" => StoreOptionsEnum.Configuration,
                    "efcore" => StoreOptionsEnum.Efcore,
                    "http-remote" => StoreOptionsEnum.HttpRemote,
                    "in-memory" => StoreOptionsEnum.InMemory,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsEfcore()
            {
                return Value == "efcore";
            }

            public bool IsHttpRemote()
            {
                return Value == "http-remote";
            }

            public bool IsConfiguration()
            {
                return Value == "configuration";
            }
        }

        public enum StoreOptionsEnum
        {
            InMemory,
            Efcore,
            HttpRemote,
            Configuration
        }

        public DataIsolationOptions DataIsolation() => new DataIsolationOptions(_groupSettings.GetSetting("be7c671e-bbef-4d75-b42d-a6547de3ae82")?.Value);

        public class DataIsolationOptions
        {
            public readonly string Value;

            public DataIsolationOptions(string value)
            {
                Value = value;
            }

            public DataIsolationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "separate-database" => DataIsolationOptionsEnum.SeparateDatabase,
                    "shared-database" => DataIsolationOptionsEnum.SharedDatabase,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSeparateDatabase()
            {
                return Value == "separate-database";
            }

            public bool IsSharedDatabase()
            {
                return Value == "shared-database";
            }
        }

        public enum DataIsolationOptionsEnum
        {
            SeparateDatabase,
            SharedDatabase,
        }
        public ApplyMultiTenancyToAggregatesOptions ApplyMultiTenancyToAggregates() => new ApplyMultiTenancyToAggregatesOptions(_groupSettings.GetSetting("d299a5bb-db30-474b-8bac-fa879151a1a3")?.Value);

        public class ApplyMultiTenancyToAggregatesOptions
        {
            public readonly string Value;

            public ApplyMultiTenancyToAggregatesOptions(string value)
            {
                Value = value;
            }

            public ApplyMultiTenancyToAggregatesOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "automatically" => ApplyMultiTenancyToAggregatesOptionsEnum.Automatically,
                    "manually" => ApplyMultiTenancyToAggregatesOptionsEnum.Manually,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsAutomatically()
            {
                return Value == "automatically";
            }

            public bool IsManually()
            {
                return Value == "manually";
            }
        }

        public enum ApplyMultiTenancyToAggregatesOptionsEnum
        {
            Automatically,
            Manually,
        }
    }
}