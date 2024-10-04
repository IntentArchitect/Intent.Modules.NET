using System;
using System.Collections.Generic;
using System.Linq;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Api;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.Utils;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates;

internal static class StoredProcedureHelpers
{
    public static void ApplyInterfaceMethods<TTemplate, TModel>(
        TTemplate template,
        IReadOnlyCollection<GeneralizedStoredProcedure> storedProcedures)
        where TTemplate : CSharpTemplateBase<TModel>, ICSharpFileBuilderTemplate
    {
        template.AddDomainTypeSources();

        template.CSharpFile
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .OnBuild(file =>
            {
                var @interface = file.Interfaces.First();

                foreach (var storedProcedure in storedProcedures)
                {
                    @interface.AddMethod(GetReturnType(template, storedProcedure), storedProcedure.InternalElement.Name.ToPascalCase(), method =>
                    {
                        method.RepresentsModel(storedProcedure.Model);
                        method.TryAddXmlDocComments(storedProcedure.InternalElement);

                        foreach (var parameter in storedProcedure.Parameters)
                        {
                            if (parameter.StoredProcedureDetails != null && parameter.StoredProcedureDetails.Direction != StoredProcedureParameterDirection.In)
                            {
                                continue;
                            }

                            method.AddParameter(template.GetTypeName(parameter.TypeReference),
                                parameter.InternalElement.Name.ToLocalVariableName());
                        }

                        method.AddOptionalCancellationTokenParameter();
                        //method.AddDeconstructedReturnMembers(GetReturnProperties(template, storedProcedure).Select(s => s.InternalElement.Name.ToCamelCase()).ToList());
                    });
                }
            });
    }

    public static void ApplyImplementationMethods<TTemplate, TModel>(
        TTemplate template,
        IReadOnlyCollection<GeneralizedStoredProcedure> storedProcedures,
        DbContextInstance dbContextInstance)
        where TTemplate : CSharpTemplateBase<TModel>, ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);

        var complexReturnTypeImplementations = new List<ComplexReturnTypeImplRecord>();

        template.CSharpFile
            .AddUsing("System.Threading")
            .AddUsing("System.Threading.Tasks")
            .OnBuild(file =>
            {
                var @class = file.Classes.First();

                foreach (var storedProcedure in storedProcedures)
                {
                    storedProcedure.Validate();

                    @class.AddMethod(GetReturnType(template, storedProcedure), storedProcedure.InternalElement.Name.ToPascalCase(), method =>
                    {
                        method.RepresentsModel(storedProcedure);

                        var returnTupleProperties = storedProcedure.Parameters
                            .Where(parameter => parameter.StoredProcedureDetails?.Direction == StoredProcedureParameterDirection.Out)
                            .Select(parameter =>
                            {
                                var sqlParameter = parameter.InternalElement.Name.ToCamelCase().EnsureSuffixedWith("Parameter");
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
                            if (parameter.StoredProcedureDetails?.Direction == StoredProcedureParameterDirection.Out)
                            {
                                continue;
                            }

                            method.AddParameter(template.GetTypeName(parameter.TypeReference),
                                parameter.InternalElement.Name.ToLocalVariableName());
                        }

                        method.AddOptionalCancellationTokenParameter();

                        var spName = !string.IsNullOrWhiteSpace(storedProcedure.Name)
                            ? storedProcedure.Name
                            : storedProcedure.InternalElement.Name;

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
                            if (parameter.StoredProcedureDetails == null)
                            {
                                continue;
                            }

                            var isOutputParameter = parameter.StoredProcedureDetails.Direction == StoredProcedureParameterDirection.Out;
                            var isUserDefinedTableType = parameter.TypeReference.Element.IsDataContractModel();
                            var output = isOutputParameter ? " OUTPUT" : string.Empty;
                            var parameterName = parameter.InternalElement.Name.ToLocalVariableName();
                            var variableName = isOutputParameter || isUserDefinedTableType || returnsScalar
                                ? parameterName.EnsureSuffixedWith("Parameter")
                                : parameterName;

                            parameters.Add((parameterName, variableName, output));

                            if (isOutputParameter)
                            {
                                method.AddStatement(parameterFactory.CreateForOutput(
                                    invocationPrefix: $"var {variableName} = ",
                                    valueVariableName: parameterName,
                                    parameter: parameter));
                            }
                            else if (returnsScalar)
                            {
                                method.AddStatement(parameterFactory.CreateForInput(
                                    invocationPrefix: $"var {variableName} = ",
                                    valueVariableName: parameterName,
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

                            AddReturnStatement(returnTupleProperties, method);
                        }
                        else if (returnTypeElement == null)
                        {
                            var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" {{{x.VariableName}}}{x.Output}"))}\"";

                            file.AddUsing("Microsoft.EntityFrameworkCore");
                            method.AddStatement($"await _dbContext.Database.ExecuteSqlInterpolatedAsync({sql}, cancellationToken);");

                            AddReturnStatement(returnTupleProperties, method);
                        }
                        else
                        {
                            var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" {{{x.VariableName}}}{x.Output}"))}\"";

                            complexReturnTypeImplementations.Add(new ComplexReturnTypeImplRecord(
                                Method: method,
                                StoredProcedure: storedProcedure,
                                Sql: sql,
                                ReturnTypeElement: returnTypeElement,
                                ResultVariableName: resultVariableName,
                                ReturnsCollection: returnsCollection,
                                ReturnTupleProperties: returnTupleProperties));
                        }


                    });
                }
            })
            .AfterBuild(file =>
            {
                var dbContextTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext, dbContextInstance);
                var dbSetPropertiesByModelId = dbContextTemplate.CSharpFile.Classes.Single().Properties
                    .Where(x => x.HasMetadata("model"))
                    .ToDictionary(x => x.GetMetadata("model") switch
                    {
                        ClassModel model => model.Id,
                        DataContractModel model => model.Id,
                        _ => throw new Exception($"Unknown type: {x.GetMetadata("model").GetType()}")
                    });

                foreach (var implRecord in complexReturnTypeImplementations)
                {
                    var source =
                        template is RepositoryTemplate repositoryTemplate &&
                        repositoryTemplate.Model.Id == implRecord.ReturnTypeElement.Id
                            ? "GetSet()"
                            : $"_dbContext.{dbSetPropertiesByModelId[implRecord.ReturnTypeElement.Id].Name}";

                    implRecord.Method.AddMethodChainStatement($"var {implRecord.ResultVariableName} = {(!implRecord.ReturnsCollection ? "(" : string.Empty)}await {source}", mcs =>
                    {
                        file.AddUsing("Microsoft.EntityFrameworkCore");

                        mcs
                            .AddChainStatement($"FromSqlInterpolated({implRecord.Sql})")
                            .AddChainStatement("IgnoreQueryFilters()")
                            .AddChainStatement($"ToArrayAsync(cancellationToken){(!implRecord.ReturnsCollection ? ")" : string.Empty)}");

                        if (!implRecord.ReturnsCollection)
                        {
                            file.AddUsing("System.Linq");
                            mcs.AddChainStatement(implRecord.StoredProcedure.TypeReference.IsNullable
                                ? "SingleOrDefault()"
                                : "Single()");
                        }

                        AddReturnStatement(implRecord.ReturnTupleProperties, implRecord.Method);
                    });
                }
            });
    }

    private static void AddReturnStatement(IReadOnlyList<string> returnTupleProperties, CSharpClassMethod method)
    {
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
    }

    private record ComplexReturnTypeImplRecord(
        CSharpClassMethod Method,
        GeneralizedStoredProcedure StoredProcedure,
        string Sql,
        ICanBeReferencedType ReturnTypeElement,
        string ResultVariableName,
        bool ReturnsCollection,
        List<string> ReturnTupleProperties);

    public static IReadOnlyCollection<StoredProcedureModel> GetStoredProcedureModels(this RepositoryModel repositoryModel)
    {
        return repositoryModel.InternalElement.ChildElements
            .Where(x => x.IsStoredProcedureModel())
            .Select(x => x.AsStoredProcedureModel())
            .ToArray();
    }

    public static void Validate(this GeneralizedStoredProcedure storedProc)
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
                    Logging.Log.Failure($"Parameter \"{parameter.InternalElement.Name}\" [{parameter.Id}] on Stored Procedure \"{storedProc.InternalElement.Name}\" [{storedProc.Id}] " +
                                        $"has \"Is Collection\" disabled and is of type \"Data Contract\", this is " +
                                        $"unsupported for user-defined table types.");
                }

                continue;
            }

            if (parameter.TypeReference.IsCollection)
            {
                Logging.Log.Failure($"Parameter \"{parameter.InternalElement.Name}\" [{parameter.Id}] on Stored Procedure \"{storedProc.InternalElement.Name}\" [{storedProc.Id}] " +
                                    $"has \"Is Collection\" enabled and is not of type \"Data Contract\", this is " +
                                    $"unsupported.");
            }
        }
    }

    private record TupleEntry(string Name, string TypeName);

    private static IReadOnlyList<TupleEntry> GetReturnProperties(ICSharpTemplate template, GeneralizedStoredProcedure storedProcedure)
    {
        var tupleProperties = storedProcedure.Parameters
            .Where(parameter => parameter.StoredProcedureDetails?.Direction == StoredProcedureParameterDirection.Out)
            .Select(parameter => new TupleEntry(
                Name: parameter.InternalElement.Name.ToPascalCase().EnsureSuffixedWith("Output"),
                TypeName: template.GetTypeName(parameter.TypeReference)
            ))
            .ToList();

        if (storedProcedure.TypeReference.Element != null)
        {
            tupleProperties.Insert(0, new TupleEntry(
                Name: storedProcedure.TypeReference.IsCollection ? "Results" : "Result",
                TypeName: template.GetTypeName(storedProcedure.TypeReference, "System.Collections.Generic.IReadOnlyCollection<{0}>")
            ));
        }

        return tupleProperties;
    }

    private static CSharpType GetReturnType(ICSharpTemplate template, GeneralizedStoredProcedure storedProcedure)
    {
        var tupleProperties = GetReturnProperties(template, storedProcedure);

        return tupleProperties.Count switch
        {
            0 => CSharpType.CreateTask(template),
            1 => CSharpType.CreateTask(new CSharpTypeName(tupleProperties[0].TypeName), template),
            _ => CSharpType.CreateTask(new CSharpTypeTuple(tupleProperties.Select(s => new CSharpTupleElement(new CSharpTypeName(s.TypeName), s.Name)).ToList()), template)
        };
    }

    private interface IDbParameterFactory
    {
        CSharpStatement CreateForOutput(string invocationPrefix,
            string valueVariableName,
            Parameter parameter);

        CSharpStatement CreateForInput(
            string invocationPrefix,
            string valueVariableName,
            Parameter parameter);

        CSharpStatement CreateForTableType(
            string invocationPrefix,
            Parameter parameter);
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