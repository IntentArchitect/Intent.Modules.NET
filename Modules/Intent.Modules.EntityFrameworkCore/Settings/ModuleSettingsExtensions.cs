using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
//using Intent.Modules.Metadata.RDBMS.Settings;
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
                    "postgresql" => DatabaseProviderOptionsEnum.Postgresql,
                    "my-sql" => DatabaseProviderOptionsEnum.MySql,
                    "cosmos" => DatabaseProviderOptionsEnum.Cosmos,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsSqlServer()
            {
                return Value == "sql-server";
            }

            public bool IsPostgresql()
            {
                return Value == "postgresql";
            }

            public bool IsMySql()
            {
                return Value == "my-sql";
            }

            public bool IsCosmos()
            {
                return Value == "cosmos";
            }
        }

        public enum DatabaseProviderOptionsEnum
        {
            InMemory,
            SqlServer,
            Postgresql,
            MySql,
            Cosmos,
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

        public static bool UseSplitQueriesByDefault(this DatabaseSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("ef908c62-0692-405f-849e-ac09c30181dd")?.Value.ToPascalCase(), out var result) && result;
    }

    //public static class ModuleSettingsExtensions
    //{
    //    public static DatabaseSettings GetDatabaseSettings(this IApplicationSettingsProvider settings)
    //    {
    //        return new DatabaseSettings(settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9"));
    //    }
    //}

    //public class DatabaseSettings : IGroupSettings
    //{
    //    private readonly IGroupSettings _groupSettings;

    //    public DatabaseSettings(IGroupSettings groupSettings)
    //    {
    //        _groupSettings = groupSettings;
    //    }

    //    public string Id => _groupSettings.Id;

    //    public string Title
    //    {
    //        get => _groupSettings.Title;
    //        set => _groupSettings.Title = value;
    //    }

    //    public ISetting GetSetting(string settingId)
    //    {
    //        return _groupSettings.GetSetting(settingId);
    //    }

    //    public KeyTypeOptions KeyType() => new KeyTypeOptions(_groupSettings.GetSetting("ef83f85d-bb8d-4b10-8842-9f35f9f54165")?.Value);

    //    public class KeyTypeOptions
    //    {
    //        public readonly string Value;

    //        public KeyTypeOptions(string value)
    //        {
    //            Value = value;
    //        }

    //        public KeyTypeOptionsEnum AsEnum()
    //        {
    //            return Value switch
    //            {
    //                "guid" => KeyTypeOptionsEnum.Guid,
    //                "long" => KeyTypeOptionsEnum.Long,
    //                "int" => KeyTypeOptionsEnum.Int,
    //                _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
    //            };
    //        }

    //        public bool IsGuid()
    //        {
    //            return Value == "guid";
    //        }

    //        public bool IsLong()
    //        {
    //            return Value == "long";
    //        }

    //        public bool IsInt()
    //        {
    //            return Value == "int";
    //        }
    //    }

    //    public enum KeyTypeOptionsEnum
    //    {
    //        Guid,
    //        Long,
    //        Int,
    //    }

    //    public KeyCreationModeOptions KeyCreationMode() => new KeyCreationModeOptions(_groupSettings.GetSetting("5aca6e0c-1b64-425b-9046-f0bc81c44311")?.Value);

    //    public class KeyCreationModeOptions
    //    {
    //        public readonly string Value;

    //        public KeyCreationModeOptions(string value)
    //        {
    //            Value = value;
    //        }

    //        public KeyCreationModeOptionsEnum AsEnum()
    //        {
    //            return Value switch
    //            {
    //                "manual" => KeyCreationModeOptionsEnum.Manual,
    //                "explicit" => KeyCreationModeOptionsEnum.Explicit,
    //                "implicit" => KeyCreationModeOptionsEnum.Implicit,
    //                _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
    //            };
    //        }

    //        public bool IsManual()
    //        {
    //            return Value == "manual";
    //        }

    //        public bool IsImplicit()
    //        {
    //            return Value == "implicit";
    //        }

    //        public bool IsExplicit()
    //        {
    //            return Value == "explicit";
    //        }
    //    }

    //    public enum KeyCreationModeOptionsEnum
    //    {
    //        Manual,
    //        Implicit,
    //        Explicit,
    //    }
    //}
}
