using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Settings
{

    public static class MultitenancySettingsExtensions
    {

        public static MongoDbDataIsolationOptions MongoDbDataIsolation(this MultitenancySettings groupSettings) => new MongoDbDataIsolationOptions(groupSettings.GetSetting("135d48e9-87cb-4380-8a3b-82e4f7553f58")?.Value);

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