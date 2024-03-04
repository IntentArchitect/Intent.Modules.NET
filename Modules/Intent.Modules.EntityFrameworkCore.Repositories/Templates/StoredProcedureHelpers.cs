using System;
using System.Collections.Generic;
using System.Linq;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Exceptions;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Utils;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates;

internal static class StoredProcedureHelpers
{
    public static void ApplyInterfaceMethods<TTemplate, TModel>(
        TTemplate template,
        IReadOnlyCollection<StoredProcedureModel> storedProcedures)
        where TTemplate : CSharpTemplateBase<TModel>, ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);

        template.CSharpFile
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AfterBuild(file =>
            {
                var @interface = file.Interfaces.First();

                foreach (var storedProcedure in storedProcedures)
                {
                    @interface.AddMethod(GetReturnType(template, storedProcedure), storedProcedure.Name.ToPascalCase(), method =>
                    {
                        method.TryAddXmlDocComments(storedProcedure.InternalElement);

                        foreach (var parameter in storedProcedure.Parameters)
                        {
                            if (parameter.GetStoredProcedureParameterSettings()?.IsOutputParameter() == true)
                            {
                                continue;
                            }

                            method.AddParameter(template.GetTypeName(parameter.TypeReference),
                                parameter.Name.ToLocalVariableName());
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", m => m.WithDefaultValue("default"));
                    });
                }
            });
    }

    public static void ApplyImplementationMethods<TTemplate, TModel>(
        TTemplate template,
        IReadOnlyCollection<StoredProcedureModel> storedProcedures)
        where TTemplate : CSharpTemplateBase<TModel>, ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);

        template.CSharpFile
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .AfterBuild(file =>
            {
                var @class = file.Classes.First();
                var dbContextTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.DbContext);
                var dbSetPropertiesByModelId = dbContextTemplate.CSharpFile.Classes.Single().Properties
                    .Where(x => x.HasMetadata("model"))
                    .ToDictionary(x => x.GetMetadata("model") switch
                    {
                        ClassModel model => model.Id,
                        DataContractModel model => model.Id,
                        _ => throw new Exception($"Unknown type: {x.GetMetadata("model").GetType()}")
                    });

                foreach (var storedProcedure in storedProcedures)
                {
                    storedProcedure.Validate();

                    @class.AddMethod(GetReturnType(template, storedProcedure), storedProcedure.Name.ToPascalCase(), method =>
                    {
                        var returnTupleProperties = storedProcedure.Parameters
                            .Where(parameter => parameter.GetStoredProcedureParameterSettings()?.IsOutputParameter() == true)
                            .Select(parameter =>
                            {
                                var sqlParameter = parameter.Name.ToCamelCase().EnsureSuffixedWith("Parameter");
                                var typeName = template.GetTypeName(parameter.TypeReference, "{0}");

                                return $"({typeName}){sqlParameter}.Value";
                            })
                            .ToList();

                        var returnsCollection = storedProcedure.TypeReference.IsCollection;
                        var resultVariableName = returnsCollection ? "results" : "result";

                        var returnTypeElement = storedProcedure.TypeReference.Element;
                        if (returnTypeElement != null)
                        {
                            returnTupleProperties.Insert(0, resultVariableName);
                        }

                        var returnsScalar = returnTypeElement?.SpecializationType == "Type-Definition";
                        if (returnsScalar && returnsCollection)
                        {
                            throw new ElementException(storedProcedure.InternalElement, "Collection of scalar return types from stored procedures is not supported");
                        }

                        method.Async();

                        foreach (var parameter in storedProcedure.Parameters)
                        {
                            if (parameter.GetStoredProcedureParameterSettings()?.IsOutputParameter() == true)
                            {
                                continue;
                            }

                            method.AddParameter(template.GetTypeName(parameter.TypeReference),
                                parameter.Name.ToLocalVariableName());
                        }

                        method.AddParameter("CancellationToken", "cancellationToken", m => m.WithDefaultValue("default"));

                        var nameInSchema = storedProcedure.GetStoredProcedureSettings()?.NameInSchema();
                        var spName = !string.IsNullOrWhiteSpace(nameInSchema)
                            ? nameInSchema
                            : storedProcedure.Name;

                        var provider = template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum();

                        IDbParameterFactory parameterFactory = provider switch
                        {
                            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory => expr,
                            DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer => new SqlDbParameterFactory(template),
                            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql => expr,
                            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql => expr,
                            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos => expr,
                            _ => throw new NotSupportedException($"{provider} is not supported at this time. Please reach out to us at " +
                                                                 $"https://github.com/IntentArchitect/Support should you need support added.")
                        };

                        var parameters = new List<(string ParameterName, string VariableName, string Output)>();

                        foreach (var parameter in storedProcedure.Parameters)
                        {
                            var isOutputParameter = parameter.GetStoredProcedureParameterSettings()?.IsOutputParameter() == true;
                            var isUserDefinedTableType = parameter.TypeReference.Element.IsDataContractModel();
                            var output = isOutputParameter ? " OUTPUT" : string.Empty;
                            var parameterName = parameter.Name.ToLocalVariableName();
                            var variableName = isOutputParameter || isUserDefinedTableType || returnsScalar
                                ? parameterName.EnsureSuffixedWith("Parameter")
                                : parameterName;

                            parameters.Add((parameterName, variableName, output));

                            if (returnsScalar)
                            {
                                method.AddStatement(parameterFactory.CreateForInput(
                                    invocationPrefix: $"var {variableName} = ",
                                    valueVariableName: parameterName,
                                    parameter: parameter));
                            }
                            else if (isOutputParameter)
                            {
                                method.AddStatement(parameterFactory.CreateForOutput(
                                    invocationPrefix: $"var {variableName} = ",
                                    parameter: parameter));
                            }
                            else if (isUserDefinedTableType)
                            {
                                method.AddStatement(parameterFactory.CreateForTableType(
                                    invocationPrefix: $"var {variableName} = ",
                                    parameter: parameter));
                            }
                        }

                        if (returnsScalar)
                        {
                            var sql = $"\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" @{x.ParameterName}{x.Output}"))}\"";

                            method.AddInvocationStatement($"var result = await _dbContext.ExecuteScalarAsync<{template.GetTypeName(storedProcedure)}>",
                                s =>
                                {
                                    s.AddArgument(sql);

                                    foreach (var parameter in parameters)
                                    {
                                        s.AddArgument($"{parameter.VariableName}");
                                    }
                                });
                        }
                        else if (returnTypeElement == null)
                        {
                            var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" {{{x.VariableName}}}{x.Output}"))}\"";

                            file.AddUsing("Microsoft.EntityFrameworkCore");
                            method.AddStatement($"await _dbContext.Database.ExecuteSqlInterpolatedAsync({sql}, cancellationToken);");
                        }
                        else
                        {
                            var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" {{{x.VariableName}}}{x.Output}"))}\"";

                            var source =
                                template is RepositoryTemplate repositoryTemplate &&
                                repositoryTemplate.Model.Id == returnTypeElement.Id
                                    ? "GetSet()"
                                    : $"_dbContext.{dbSetPropertiesByModelId[returnTypeElement.Id].Name}";

                            method.AddMethodChainStatement($"var {resultVariableName} = {(!returnsCollection ? "(" : string.Empty)}await {source}", mcs =>
                            {
                                file.AddUsing("Microsoft.EntityFrameworkCore");

                                mcs
                                    .AddChainStatement($"FromSqlInterpolated({sql})")
                                    .AddChainStatement("IgnoreQueryFilters()")
                                    .AddChainStatement($"ToArrayAsync(cancellationToken){(!returnsCollection ? ")" : string.Empty)}");

                                if (!returnsCollection)
                                {
                                    file.AddUsing("System.Linq");
                                    mcs.AddChainStatement(storedProcedure.TypeReference.IsNullable
                                        ? "SingleOrDefault()"
                                        : "Single()");
                                }
                            });
                        }

                        switch (returnTupleProperties.Count)
                        {
                            case 0:
                                return;
                            case 1:
                                method.AddStatement($"return {returnTupleProperties[0]};", s => s.SeparatedFromPrevious());
                                return;
                            case > 1:
                                method.AddStatement($"return ({string.Join(", ", returnTupleProperties)});", s => s.SeparatedFromPrevious());
                                return;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    });
                }
            });
    }

    public static IReadOnlyCollection<StoredProcedureModel> GetStoredProcedureModels(this RepositoryModel repositoryModel)
    {
        return repositoryModel.InternalElement.ChildElements
            .Where(x => x.IsStoredProcedureModel())
            .Select(x => x.AsStoredProcedureModel())
            .ToArray();
    }

    public static void Validate(this StoredProcedureModel storedProc)
    {
        foreach (var parameter in storedProc.Parameters)
        {
            var dataContract = parameter.TypeReference.Element.AsDataContractModel();
            if (dataContract != null)
            {
                foreach (var attribute in dataContract.Attributes)
                {
                    if (attribute.TypeReference.IsCollection)
                    {
                        Logging.Log.Failure($"Attribute \"{attribute.Name}\" [{attribute.Id}] on Data Contract \"{dataContract.Name}\" [{dataContract.Id}] " +
                                            $"has \"Is Collection\" enabled and is used as a Stored Procedure Parameter, this is " +
                                            $"unsupported for user-defined table types.");
                    }

                    if (!attribute.TypeReference.Element.IsTypeDefinitionModel())
                    {
                        Logging.Log.Failure($"Attribute \"{attribute.Name}\" [{attribute.Id}] on Data Contract \"{dataContract.Name}\" [{dataContract.Id}] " +
                                            $"has type non-\"Type Definition\" type \"{attribute.TypeReference.Element.SpecializationType}\" " +
                                            $"and is used as a Stored Procedure Parameter, this is unsupported for user-defined table types.");
                    }
                }

                if (!parameter.TypeReference.IsCollection)
                {
                    Logging.Log.Failure($"Parameter \"{parameter.Name}\" [{parameter.Id}] on Stored Procedure \"{storedProc.Name}\" [{storedProc.Id}] " +
                                        $"has \"Is Collection\" disabled and is of type \"Data Contract\", this is " +
                                        $"unsupported for user-defined table types.");
                }

                continue;
            }

            if (parameter.TypeReference.IsCollection)
            {
                Logging.Log.Failure($"Parameter \"{parameter.Name}\" [{parameter.Id}] on Stored Procedure \"{storedProc.Name}\" [{storedProc.Id}] " +
                                    $"has \"Is Collection\" enabled and is not of type \"Data Contract\", this is " +
                                    $"unsupported.");
            }
        }
    }

    private static string GetReturnType(IntentTemplateBase template, StoredProcedureModel storedProcedure)
    {
        var tupleProperties = storedProcedure.Parameters
            .Where(parameter => parameter.GetStoredProcedureParameterSettings()?.IsOutputParameter() == true)
            .Select(parameter => new
            {
                Name = parameter.Name.ToPascalCase().EnsureSuffixedWith("Output"),
                TypeName = template.GetTypeName(parameter)
            })
            .ToList();

        if (storedProcedure.TypeReference.Element != null)
        {
            tupleProperties.Insert(0, new
            {
                Name = storedProcedure.TypeReference.IsCollection ? "Results" : "Result",
                TypeName = template.GetTypeName(storedProcedure.TypeReference, "System.Collections.Generic.IReadOnlyCollection<{0}>")
            });
        }

        return tupleProperties.Count switch
        {
            0 => "Task",
            1 => $"Task<{tupleProperties[0].TypeName}>",
            > 1 => $"Task<({string.Join(", ", tupleProperties.Select(x => $"{x.TypeName} {x.Name}"))})>",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private interface IDbParameterFactory
    {
        CSharpStatement CreateForOutput(
            string invocationPrefix,
            StoredProcedureParameterModel parameter);

        CSharpStatement CreateForInput(
            string invocationPrefix,
            string valueVariableName,
            StoredProcedureParameterModel parameter);

        CSharpStatement CreateForTableType(
            string invocationPrefix,
            StoredProcedureParameterModel parameter);
    }

    /// <summary>
    /// Microsoft SQL Server implementation of <see cref="IDbParameterFactory"/>.
    /// </summary>
    private class SqlDbParameterFactory : IDbParameterFactory
    {
        private readonly ICSharpFileBuilderTemplate _template;
        private string _parameterTypeName;
        private string _parameterDirectionTypeName;
        private string _dbTypeTypeName;

        public SqlDbParameterFactory(ICSharpFileBuilderTemplate template)
        {
            _template = template;
        }

        private string ParameterTypeName => _parameterTypeName ??= _template.UseType("Microsoft.Data.SqlClient.SqlParameter");
        private string ParameterDirectionTypeName => _parameterDirectionTypeName ??= _template.UseType("System.Data.ParameterDirection");
        private string DbTypeTypeName => _dbTypeTypeName ??= _template.UseType("System.Data.SqlDbType");

        public CSharpStatement CreateForOutput(
            string invocationPrefix,
            StoredProcedureParameterModel parameter)
        {
            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("Direction", $"{ParameterDirectionTypeName}.Output");
            statement.AddObjectInitStatement("SqlDbType", $"{DbTypeTypeName}.{GetSqlDbType(parameter)}");
            statement.WithSemicolon();

            return statement;
        }

        public CSharpStatement CreateForInput(
            string invocationPrefix,
            string valueVariableName,
            StoredProcedureParameterModel parameter)
        {
            var statement = new CSharpObjectInitializerBlock($"{invocationPrefix}new {ParameterTypeName}");

            statement.AddObjectInitStatement("Direction", $"{ParameterDirectionTypeName}.Input");
            statement.AddObjectInitStatement("SqlDbType", $"{DbTypeTypeName}.{GetSqlDbType(parameter)}");
            statement.AddObjectInitStatement("ParameterName", $"\"{valueVariableName}\"");
            statement.AddObjectInitStatement("Value", valueVariableName);
            statement.WithSemicolon();

            return statement;
        }

        public CSharpStatement CreateForTableType(
            string invocationPrefix,
            StoredProcedureParameterModel parameter)
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
            statement.AddObjectInitStatement("Value", $"{parameter.Name.ToLocalVariableName()}.ToDataTable()");
            statement.AddObjectInitStatement("TypeName", $"\"{userDefinedTableName}\"");
            statement.WithSemicolon();

            return statement;
        }

        private static string GetSqlDbType(StoredProcedureParameterModel parameter)
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
                "string" => "VarChar",
                _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter.TypeReference.Element.Name, null)
            };
        }
    }
}