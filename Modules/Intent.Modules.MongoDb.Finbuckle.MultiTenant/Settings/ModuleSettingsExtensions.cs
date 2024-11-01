using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.AspNetCore.MultiTenancy.Settings;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Finbuckle.MultiTenant.Settings
{

    public static class MultitenancySettingsExtensions
    {
        public static MongoDbDataIsolationOptions MongoDbDataIsolation(this MultitenancySettings groupSettings) => new MongoDbDataIsolationOptions(groupSettings.GetSetting("2224fa66-e2dc-467f-83a6-33abcd233a46")?.Value);

        public class MongoDbDataIsolationOptions
        {
            public readonly string Value;

            public MongoDbDataIsolationOptions(string value)
            {
                Value = value;
            }

            public MongoDbDataIsolationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => MongoDbDataIsolationOptionsEnum.None,
                    "separate-database" => MongoDbDataIsolationOptionsEnum.SeparateDatabase,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsSeparateDatabase()
            {
                return Value == "separate-database";
            }
        }

        public enum MongoDbDataIsolationOptionsEnum
        {
            None,
            SeparateDatabase,
        }
    }
}