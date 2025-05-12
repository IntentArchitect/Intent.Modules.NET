using System;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories.Interfaces;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories
{
    /// <summary>
    /// Microsoft SQL Server implementation of <see cref="IDbParameterFactory"/>.
    /// </summary>
    internal class SqlDbParameterFactory : IDbParameterFactory
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private string? _parameterTypeName;
        private string? _parameterDirectionTypeName;
        private string? _dbTypeTypeName;

        public SqlDbParameterFactory(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        private string ParameterTypeName => _parameterTypeName ??= _template.UseType("Microsoft.Data.SqlClient.SqlParameter");
        private string ParameterDirectionTypeName => _parameterDirectionTypeName ??= _template.UseType("System.Data.ParameterDirection");
        private string DbTypeTypeName => _dbTypeTypeName ??= _template.UseType("System.Data.SqlDbType");

        public CSharpStatement CreateForOutput(string invocationPrefix,
            string valueVariableName,
            Parameter parameter)
        {
            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("Direction", $"{ParameterDirectionTypeName}.Output");
            statement.AddObjectInitStatement("SqlDbType", $"{DbTypeTypeName}.{GetSqlDbType(parameter)}");
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
                    // Built-in defaults for EF SQL Server
                    precision ??= 18;
                    scale ??= 2;
                }

                statement.AddObjectInitStatement("Precision", precision.ToString());
                statement.AddObjectInitStatement("Scale", scale.ToString());
            }
            statement.AddObjectInitStatement("ParameterName", $"\"@{valueVariableName}\"");
            statement.WithSemicolon();

            return statement;
        }

        public CSharpStatement CreateForInput(
            string invocationPrefix,
            string valueVariableName,
            Parameter parameter)
        {
            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("Direction", $"{ParameterDirectionTypeName}.Input");
            statement.AddObjectInitStatement("SqlDbType", $"{DbTypeTypeName}.{GetSqlDbType(parameter)}");
            statement.AddObjectInitStatement("ParameterName", $"\"@{valueVariableName}\"");
            statement.AddObjectInitStatement("Value", valueVariableName);
            statement.WithSemicolon();

            return statement;
        }

        public CSharpStatement CreateForTableType(
            string invocationPrefix,
            Parameter parameter)
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
            statement.AddObjectInitStatement("SqlDbType", $"{DbTypeTypeName}.Structured");
            statement.AddObjectInitStatement("Value", $"{parameter.InternalElement.Name.ToLocalVariableName()}.ToDataTable()");
            statement.AddObjectInitStatement("TypeName", $"\"{userDefinedTableName}\"");
            statement.WithSemicolon();

            return statement;
        }

        private static string GetSqlDbType(Parameter parameter)
        {
            // https://learn.microsoft.com/dotnet/framework/data/adonet/sql-server-data-type-mappings
            return parameter.TypeReference.Element.Name.ToLowerInvariant() switch
            {
                "binary" => "VarBinary",
                "bool" => "Bit",
                "byte" => "TinyInt",
                "date" => "Date",
                "datetime" => "DateTime2",
                "datetimeoffset" => "DateTimeOffset",
                "decimal" => "Decimal",
                "double" => "Float",
                "float" => "Real",
                "guid" => "UniqueIdentifier",
                "int" => "Int",
                "long" => "BigInt",
                "short" => "SmallInt",
                "string" => GetStringSqlType(parameter),
                _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter.TypeReference.Element.Name, null)
            };
        }

        private static string GetStringSqlType(Parameter parameter)
        {
            var sqlStringType = parameter.StoredProcedureDetails?.SqlStringType;
            return sqlStringType switch
            {
                null => "VarChar",
                var value => value
            };
        }
    }
}
