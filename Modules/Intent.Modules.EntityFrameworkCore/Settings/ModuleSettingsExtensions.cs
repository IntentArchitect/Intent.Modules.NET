using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Settings
{

    public static class DatabaseSettingsExtensions
    {

        public static DatabaseProviderOptions DatabaseProvider(this DatabaseSettings groupSettings) => new DatabaseProviderOptions(groupSettings.GetSetting("00bb780c-57bf-43c1-b952-303f11096be7")?.Value);

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
                    "sql-server" => DatabaseProviderOptionsEnum.SqlServer,
                    "cosmos" => DatabaseProviderOptionsEnum.Cosmos,
                    "postgresql" => DatabaseProviderOptionsEnum.Postgresql,
                    "my-sql" => DatabaseProviderOptionsEnum.MySql,
                    "oracle" => DatabaseProviderOptionsEnum.Oracle,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsCosmos()
            {
                return Value == "cosmos";
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsMySql()
            {
                return Value == "my-sql";
            }

            public bool IsOracle()
            {
                return Value == "oracle";
            }

            public bool IsPostgresql()
            {
                return Value == "postgresql";
            }

            public bool IsSqlServer()
            {
                return Value == "sql-server";
            }
        }

        public enum DatabaseProviderOptionsEnum
        {
            InMemory,
            SqlServer,
            Postgresql,
            MySql,
            Cosmos,
            Oracle,
        }

        public static TableNamingConventionOptions TableNamingConvention(this DatabaseSettings groupSettings) => new TableNamingConventionOptions(groupSettings.GetSetting("49b09c68-e86a-4e15-96ab-cd482168ef22")?.Value);

        public class TableNamingConventionOptions
        {
            public readonly string Value;

            public TableNamingConventionOptions(string value)
            {
                Value = value;
            }

            public TableNamingConventionOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "pluralized" => TableNamingConventionOptionsEnum.Pluralized,
                    "singularized" => TableNamingConventionOptionsEnum.Singularized,
                    "none" => TableNamingConventionOptionsEnum.None,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsPluralized()
            {
                return Value == "pluralized";
            }

            public bool IsSingularized()
            {
                return Value == "singularized";
            }

            public bool IsNone()
            {
                return Value == "none";
            }
        }

        public enum TableNamingConventionOptionsEnum
        {
            Pluralized,
            Singularized,
            None,
        }

        public static bool LazyLoadingWithProxies(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("182cdc53-baee-4bb3-adbd-d2a0aa1216a1")?.Value.ToPascalCase(), out var result) && result;

        public static bool GenerateDbContextInterface(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("85dea0e8-8981-4c7b-908e-d99294fc37f1")?.Value.ToPascalCase(), out var result) && result;

        public static bool EnableSplitQueriesGlobally(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("ef908c62-0692-405f-849e-ac09c30181dd")?.Value.ToPascalCase(), out var result) && result;

        public static bool StoreEnumsAsStrings(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("df567ad2-98a7-49ce-9952-4a26b6074410")?.Value.ToPascalCase(), out var result) && result;

        public static bool EnumCheckConstraints(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("e396f58c-f184-48b1-bf36-399af463c3c1")?.Value.ToPascalCase(), out var result) && result;

        public static string DefaultSchemaName(this DatabaseSettings groupSettings) => groupSettings.GetSetting("7e0f472d-6cf0-423e-872a-cd6b2e0614bc")?.Value;

        public static bool MaintainColumnOrdering(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("1cb44856-03a7-4a0c-88cf-ae9f84b9dd79")?.Value.ToPascalCase(), out var result) && result;
    }
}
