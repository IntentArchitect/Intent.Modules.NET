using Intent.Modules.Metadata.RDBMS.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AspNetCore.IntegrationTesting.Templates
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
    }
}
