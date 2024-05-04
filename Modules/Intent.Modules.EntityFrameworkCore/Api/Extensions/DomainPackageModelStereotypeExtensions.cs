using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.EntityFrameworkCore.Api
{
    public static class DomainPackageModelStereotypeExtensions
    {
        public static CosmosDBContainerSettings GetCosmosDBContainerSettings(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype("b4995259-b47b-405a-a332-fd3dc69cd3a5");
            return stereotype != null ? new CosmosDBContainerSettings(stereotype) : null;
        }


        public static bool HasCosmosDBContainerSettings(this DomainPackageModel model)
        {
            return model.HasStereotype("b4995259-b47b-405a-a332-fd3dc69cd3a5");
        }

        public static bool TryGetCosmosDBContainerSettings(this DomainPackageModel model, out CosmosDBContainerSettings stereotype)
        {
            if (!HasCosmosDBContainerSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new CosmosDBContainerSettings(model.GetStereotype("b4995259-b47b-405a-a332-fd3dc69cd3a5"));
            return true;
        }

        public static DatabaseSettings GetDatabaseSettings(this DomainPackageModel model)
        {
            var stereotype = model.GetStereotype("897cc466-d518-444c-bb01-769024eee290");
            return stereotype != null ? new DatabaseSettings(stereotype) : null;
        }


        public static bool HasDatabaseSettings(this DomainPackageModel model)
        {
            return model.HasStereotype("897cc466-d518-444c-bb01-769024eee290");
        }

        public static bool TryGetDatabaseSettings(this DomainPackageModel model, out DatabaseSettings stereotype)
        {
            if (!HasDatabaseSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new DatabaseSettings(model.GetStereotype("897cc466-d518-444c-bb01-769024eee290"));
            return true;
        }

        public class CosmosDBContainerSettings
        {
            private IStereotype _stereotype;

            public CosmosDBContainerSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ContainerName()
            {
                return _stereotype.GetProperty<string>("Container Name");
            }

            public string PartitionKey()
            {
                return _stereotype.GetProperty<string>("Partition Key");
            }

        }

        public class DatabaseSettings
        {
            private IStereotype _stereotype;

            public DatabaseSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string ConnectionStringName()
            {
                return _stereotype.GetProperty<string>("Connection String Name");
            }

            public DatabaseProviderOptions DatabaseProvider()
            {
                return new DatabaseProviderOptions(_stereotype.GetProperty<string>("Database Provider"));
            }

            public class DatabaseProviderOptions
            {
                public readonly string Value;

                public DatabaseProviderOptions(string value)
                {
                    Value = value;
                }

                public DatabaseProviderOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "SQL Server":
                            return DatabaseProviderOptionsEnum.SQLServer;
                        case "PostgreSQL":
                            return DatabaseProviderOptionsEnum.PostgreSQL;
                        case "MySQL":
                            return DatabaseProviderOptionsEnum.MySQL;
                        case "Oracle":
                            return DatabaseProviderOptionsEnum.Oracle;
                        case "In Memory":
                            return DatabaseProviderOptionsEnum.InMemory;
                        case "Default":
                            return DatabaseProviderOptionsEnum.Default;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsSQLServer()
                {
                    return Value == "SQL Server";
                }
                public bool IsPostgreSQL()
                {
                    return Value == "PostgreSQL";
                }
                public bool IsMySQL()
                {
                    return Value == "MySQL";
                }
                public bool IsOracle()
                {
                    return Value == "Oracle";
                }
                public bool IsInMemory()
                {
                    return Value == "In Memory";
                }
                public bool IsDefault()
                {
                    return Value == "Default";
                }
            }

            public enum DatabaseProviderOptionsEnum
            {
                SQLServer,
                PostgreSQL,
                MySQL,
                Oracle,
                InMemory,
                Default
            }
        }

    }
}