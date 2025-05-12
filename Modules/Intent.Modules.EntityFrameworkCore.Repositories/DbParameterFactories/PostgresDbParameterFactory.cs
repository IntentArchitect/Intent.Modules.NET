using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories.Interfaces;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Templates;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories
{
    internal class PostgresDbParameterFactory : IDbParameterFactory
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private string _parameterTypeName;
        private string _parameterDirectionTypeName;
        private string _dbTypeTypeName;

        public PostgresDbParameterFactory(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        private string ParameterTypeName => _parameterTypeName ??= _template.UseType("Npgsql.NpgsqlParameter");
        private string ParameterDirectionTypeName => _parameterDirectionTypeName ??= _template.UseType("System.Data.ParameterDirection");
        private string DbTypeTypeName => _dbTypeTypeName ??= _template.UseType("NpgsqlTypes.NpgsqlDbType");

        public CSharpStatement CreateForInput(string invocationPrefix, string valueVariableName, Parameter parameter)
        {
            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("Direction", $"{ParameterDirectionTypeName}.Input");
            statement.AddObjectInitStatement("NpgsqlDbType", $"{DbTypeTypeName}.{GetPostgresSqlDbType(parameter)}");
            statement.AddObjectInitStatement("ParameterName", $"\"{valueVariableName}\"");
            statement.AddObjectInitStatement("Value", valueVariableName);
            statement.WithSemicolon();

            return statement;
        }

        public CSharpStatement CreateForOutput(string invocationPrefix, string valueVariableName, Parameter parameter)
        {
            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("Direction", $"{ParameterDirectionTypeName}.InputOutput");
            statement.AddObjectInitStatement("NpgsqlDbType", $"{DbTypeTypeName}.{GetPostgresSqlDbType(parameter)}");
            if (parameter.TypeReference.HasStringType())
            {
                var size = parameter.StoredProcedureDetails?.Size;
                if (size.HasValue)
                {
                    statement.AddObjectInitStatement("Size", size.ToString());
                }
            }
            if (parameter.TypeReference.HasDecimalType())
            {
                var precision = parameter.StoredProcedureDetails?.Precision;
                var scale = parameter.StoredProcedureDetails?.Scale;
                if (_template.ExecutionContext.Settings.GetDatabaseSettings().TryGetDecimalPrecisionAndScale(out var constraints))
                {
                    precision ??= constraints.Precision;
                    scale ??= constraints.Scale;
                }
                else
                {
                    // EF Core with PostgreSQL doesn't need defaults; but for compatibility:
                    precision ??= null;
                    scale ??= 0;
                }

                statement.AddObjectInitStatement("Precision", precision.ToString());
                statement.AddObjectInitStatement("Scale", scale.ToString());
            }
            statement.AddObjectInitStatement("ParameterName", $"\"{valueVariableName}\"");
            statement.AddObjectInitStatement("Value", "DBNull.Value");
            statement.WithSemicolon();

            return statement;
        }

        public CSharpStatement CreateForTableType(string invocationPrefix, Parameter parameter)
        {
            var dataContractModel = parameter.TypeReference.Element.AsDataContractModel();
            var userDefinedTableName = dataContractModel.GetUserDefinedTableTypeSettings()?.Name();
            if (string.IsNullOrWhiteSpace(userDefinedTableName))
            {
                userDefinedTableName = dataContractModel.Name;
            }

            // Add using for the extension method:
            _template.GetDataContractExtensionMethodsName(dataContractModel);

            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("IsNullable", parameter.TypeReference.IsNullable ? "true" : "false");
            statement.AddObjectInitStatement("NpgsqlDbType", $"{DbTypeTypeName}.Unknown");
            statement.AddObjectInitStatement("Value", $"{parameter.InternalElement.Name.ToLocalVariableName()}.ToDataTable()");
            statement.AddObjectInitStatement("TypeName", $"\"{userDefinedTableName}\"");
            statement.WithSemicolon();

            return statement;
        }


        private static string GetPostgresSqlDbType(Parameter parameter)
        {
            // https://www.npgsql.org/doc/api/NpgsqlTypes.NpgsqlDbType.html
            return parameter.TypeReference.Element.Name.ToLowerInvariant() switch
            {
                "binary" => "Bytea",
                "bool" => "Boolean",
                "byte" => "Smallint",
                "date" or "datetime" or "datetimeoffset" => "Date",
                "decimal" => "Numeric",
                "double" => "Double",
                "float" => "Real",
                "guid" => "Uuid",
                "int" => "Integer",
                "long" => "Bigint",
                "short" => "Smallint",
                "string" => GetPostgresStringSqlType(parameter),
                _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter.TypeReference.Element.Name, null)
            };
        }

        private static string GetPostgresStringSqlType(Parameter parameter)
        {
            var sqlStringType = parameter.StoredProcedureDetails?.SqlStringType;

            return sqlStringType?.ToLowerInvariant() switch
            {
                null => "Text", // Default in PostgreSQL
                "varchar" or "nvarchar" => "Varchar",
                "text" => "Text",
                "char" => "Char",
                _ => "Text" // fallback for unrecognized types
            };
        }
    }
}
