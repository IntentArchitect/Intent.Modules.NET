using Intent.Configuration;
using Intent.Engine;

namespace Intent.Modules.Eventing.Wolverine.Settings
{
    internal static class DatabaseProviderExtensions
    {
        public static IGroupSettings GetDatabaseSettings(this IApplicationSettingsProvider settings)
        {
            return settings.GetGroup("ac0a788e-d8b3-4eea-b56d-538608f1ded9");
        }

        public static DatabaseProviderOptions DatabaseProvider(this IGroupSettings groupSettings)
        {
            return new DatabaseProviderOptions(groupSettings.GetSetting("00bb780c-57bf-43c1-b952-303f11096be7")?.Value);
        }

        // Same "Default Connection String Name" setting Intent.Modules.EntityFrameworkCore reads via
        // DatabaseSettings.DefaultConnectionStringName() - mirrored here rather than referenced, to
        // avoid a compile dependency on that module. Blank resolves to "DefaultConnection", matching
        // DbContextInstance.ResolveDefaultConnectionStringName's fallback.
        public static string ConnectionStringName(this IGroupSettings groupSettings)
        {
            var value = groupSettings.GetSetting("ad9681ea-9388-4415-9b94-de2ced2b7307")?.Value;
            return string.IsNullOrWhiteSpace(value) ? "DefaultConnection" : value;
        }

        public class DatabaseProviderOptions
        {
            public readonly string? Value;

            public DatabaseProviderOptions(string? value)
            {
                Value = value;
            }

            public bool IsSqlServer() => Value == "sql-server";

            public bool IsPostgresql() => Value == "postgresql";

            public bool IsSupported() => IsSqlServer() || IsPostgresql();
        }
    }
}
