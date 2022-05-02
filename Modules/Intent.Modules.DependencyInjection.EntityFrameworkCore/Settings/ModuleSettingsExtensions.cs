using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.DependencyInjection.EntityFrameworkCore.Settings
{

    public static class DatabaseSettingsExtensions
    {

        public static DatabaseProviderOptions DatabaseProvider(this DatabaseSettings groupSettings) => new DatabaseProviderOptions(groupSettings.GetSetting("a6441c16-5a85-44be-be0e-118237d45ba0")?.Value);

        public class DatabaseProviderOptions
        {
            public readonly string Value;

            public DatabaseProviderOptions(string value)
            {
                Value = value;
            }

            public DatabaseProviderOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "in-memory" => DatabaseProviderOptionsEnum.InMemory,
                    "sql-server" => DatabaseProviderOptionsEnum.SQLServer,
                    "postgresql" => DatabaseProviderOptionsEnum.PostgreSQL,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsSQLServer()
            {
                return Value == "sql-server";
            }

            public bool IsPostgreSQL()
            {
                return Value == "postgresql";
            }
        }

        public enum DatabaseProviderOptionsEnum
        {
            InMemory,
            SQLServer,
            PostgreSQL,
        }
    }
}