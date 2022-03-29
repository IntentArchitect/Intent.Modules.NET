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

    public class MultitenancySettings
    {
        private readonly IGroupSettings _groupSettings;

        public MultitenancySettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
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
                    "header" => StrategyOptionsEnum.HeaderStrategy,
                    "claim" => StrategyOptionsEnum.ClaimStrategy,
                    "host" => StrategyOptionsEnum.HostStrategy,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsHeaderStrategy()
            {
                return Value == "header";
            }

            public bool IsClaimStrategy()
            {
                return Value == "claim";
            }

            public bool IsHostStrategy()
            {
                return Value == "host";
            }
        }

        public enum StrategyOptionsEnum
        {
            HeaderStrategy,
            ClaimStrategy,
            HostStrategy
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
                    "in-memory" => StoreOptionsEnum.InMemory,
                    "efcore" => StoreOptionsEnum.EntityFrameworkCore,
                    "configuration" => StoreOptionsEnum.Configuration,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsEntityFrameworkCore()
            {
                return Value == "efcore";
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsConfiguration()
            {
                return Value == "configuration";
            }
        }

        public enum StoreOptionsEnum
        {
            EntityFrameworkCore,
            InMemory,
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
                    "separate-database" => DataIsolationOptionsEnum.SeparateDatabases,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsSeparateDatabases()
            {
                return Value == "separate-database";
            }
        }

        public enum DataIsolationOptionsEnum
        {
            SeparateDatabases,
        }
    }
}