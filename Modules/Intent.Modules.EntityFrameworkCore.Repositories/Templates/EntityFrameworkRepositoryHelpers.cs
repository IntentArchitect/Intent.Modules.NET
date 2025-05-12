#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.EntityFrameworkCore.Api;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Constants;
using Intent.Modules.EntityFrameworkCore.Repositories.Api;
using Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories;
using Intent.Modules.EntityFrameworkCore.Repositories.DbParameterFactories.Interfaces;
using Intent.Modules.EntityFrameworkCore.Repositories.Templates.Repository;
using Intent.Modules.EntityFrameworkCore.Settings;
using Intent.Modules.EntityFrameworkCore.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.Utils;
using static Intent.Modules.Constants.TemplateRoles.Blazor.Client;
using static Intent.Modules.Constants.TemplateRoles.Repository;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates;

internal static class EntityFrameworkRepositoryHelpers
{
    public static void ApplyEFInterfaceMethods<TTemplate, TModel>(
        TTemplate template,
        RepositoryModel repositoryModel)
        where TTemplate : ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);

        template.CSharpFile
            .OnBuild(file =>
            {
                var @interface = file.Interfaces.First();

                foreach (var childElement in repositoryModel.InternalElement.ChildElements)
                {
                    var operationModel = OperationModelExtensions.AsOperationModel(childElement);
                    if (operationModel != null)
                    {
                        var interfaceMethod = @interface.Methods.First(m => m.Name == operationModel.Name.ToPascalCase());

                        var hasStoredProcStereotype = false;
                        if (operationModel.TryGetStoredProcedure(out var stereotype))
                        {
                            interfaceMethod.WithReturnType(GetStoredProcedureReturnType(template, new GeneralizedStoredProcedure(operationModel, stereotype)));
                            hasStoredProcStereotype = true;
                        }

                        if (interfaceMethod != null && (operationModel.StoredProcedureInvocationTargets().Any(x => x.TypeReference.Element?.IsStoredProcedureModel() == true)
                            || hasStoredProcStereotype))
                        {
                            template.CSharpFile.AddUsing("System.Threading");
                            template.CSharpFile.AddUsing("System.Threading.Tasks");
                            interfaceMethod.Async();
                            interfaceMethod.AddOptionalCancellationTokenParameter();
                        }
                    }

                    var storedProcedureModel = childElement.AsStoredProcedureModel();
                    if (storedProcedureModel != null)
                    {
                        var generalizedStoredProcedure = new GeneralizedStoredProcedure(storedProcedureModel);
                        var returnType = GetStoredProcedureReturnType(template, generalizedStoredProcedure);

                        @interface.AddMethod(returnType, storedProcedureModel.Name.ToPascalCase(), method =>
                        {
                            method.TryAddXmlDocComments(childElement);
                            method.AddMetadata("model", storedProcedureModel);
                            method.RepresentsModel(storedProcedureModel);

                            foreach (var parameter in generalizedStoredProcedure.Parameters.Where(x => x.StoredProcedureDetails.Direction
                                         is StoredProcedureParameterDirection.In
                                         or StoredProcedureParameterDirection.Both))
                            {
                                method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.InternalElement.Name.ToLocalVariableName());
                            }

                            template.CSharpFile.AddUsing("System.Threading");
                            template.CSharpFile.AddUsing("System.Threading.Tasks");
                            method.Async();
                            method.AddOptionalCancellationTokenParameter();
                        });
                    }
                }
            }, 5);
    }

    /// <summary>
    /// Apply any custom repository methods to the first class of the provided <paramref name="template"/>.
    /// </summary>
    public static void ApplyEFImplementationMethods<TTemplate, TModel>(
        TTemplate template,
        RepositoryModel repositoryModel)
       where TTemplate : ICSharpFileBuilderTemplate
    {
        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);

        var dbContextInstance = new DbContextInstance(repositoryModel.InternalElement.Package.AsDomainPackageModel());
        var dbcontextName = dbContextInstance.GetTypeName(template);

        // The .Value of this must only be called from AfterBuild when the DbContext file has been built
        var dbSetPropertiesByModelId = new Lazy<Dictionary<string, CSharpProperty>>(() =>
        {
            var dbContextTemplate = template.GetTemplate<ICSharpFileBuilderTemplate>(TemplateRoles.Infrastructure.Data.ConnectionStringDbContext, dbContextInstance);
            return dbContextTemplate.CSharpFile.Classes.Single().Properties
                .Where(x => x.HasMetadata("model"))
                .ToDictionary(x => x.GetMetadata("model") switch
                {
                    ClassModel model => model.Id,
                    DataContractModel model => model.Id,
                    _ => throw new Exception($"Unknown type: {x.GetMetadata("model").GetType()}")
                });
        });

        var @class = template.CSharpFile.Classes.First();

        // if its a non-entity repository, then add the constructor
        if (repositoryModel.TypeReference.Element == null)
        {
            @class.AddConstructor(ctor =>
            {
                if (!string.IsNullOrWhiteSpace(dbcontextName))
                {
                    ctor.AddParameter(dbcontextName, "dbContext", p => p.IntroduceReadonlyField());
                }
            });
        }

        foreach (var childElement in repositoryModel.InternalElement.ChildElements)
        {
            var operationModel = OperationModelExtensions.AsOperationModel(childElement);
            if (operationModel != null)
            {
                template.CSharpFile.OnBuild(file =>
                {
                    var method = @class.Methods.First(m => m.Name == operationModel.Name.ToPascalCase());

                    var returnType = operationModel.ReturnType is null ? "void" : template.GetTypeName(operationModel.ReturnType, "System.Collections.Generic.List<{0}>");
                    var hasStoredProcStereotype = false;
                    if (operationModel.TryGetStoredProcedure(out var stereotype))
                    {
                        method.WithReturnType(GetStoredProcedureReturnType(template, new GeneralizedStoredProcedure(operationModel, stereotype)));
                        hasStoredProcStereotype = true;
                    }

                    if (method != null && (operationModel.StoredProcedureInvocationTargets().Any(x => x.TypeReference.Element?.IsStoredProcedureModel() == true)
                        || hasStoredProcStereotype))
                    {
                        template.CSharpFile.AddUsing("System.Threading");
                        template.CSharpFile.AddUsing("System.Threading.Tasks");
                        method.Async();
                        method.AddOptionalCancellationTokenParameter();
                    }

                    if (TryGetMappedStoredProcedure(
                            operationModel: operationModel,
                            template: template,
                            storedProcedure: out var generalizedStoredProcedure,
                            parameterIdToSourceExpressions: out var parameterIdToSourceExpressions))
                    {
                        var hasReturnType = operationModel.ReturnType != null;

                        ApplyStoredProcedureImplementation(
                            template: template,
                            method: method,
                            storedProcedure: generalizedStoredProcedure,
                            sourceExpressionsByParameterId: parameterIdToSourceExpressions,
                            dbSetPropertiesByModelId: dbSetPropertiesByModelId,
                            applyReturnStatement: false,
                            assignResultToVariable: hasReturnType,
                            resultExpressionsByModel: out var resultExpressionsByModel);

                        if (hasReturnType)
                        {
                            var resultStaticElement = new StaticMetadata("1eba9280-3bf0-46f8-981c-414dee8e35c3");
                            var mappingManager = new CSharpClassMappingManager(template);
                            mappingManager.AddMappingResolver(new MappingResolver(template));

                            var targetEndModel = operationModel.StoredProcedureInvocationTargets().First();
                            var resultMapping = targetEndModel.GetMapResultMapping();

                            foreach (var (model, expression) in resultExpressionsByModel)
                            {
                                var type = model.Id == generalizedStoredProcedure.Id
                                    ? resultStaticElement
                                    : model;

                                mappingManager.SetFromReplacement(type, expression);
                            }

                            var generateTargetStatementForMapping = mappingManager.GenerateCreationStatement(resultMapping);
                            method.AddStatement(new CSharpReturnStatement(generateTargetStatementForMapping), s => s.SeparatedFromPrevious());
                        }

                        return;
                    }

                    if (TryGetStereotypeStoredProcedure(
                            operationModel: operationModel,
                            storedProcedure: out generalizedStoredProcedure,
                            parameterIdToSourceExpressions: out parameterIdToSourceExpressions))
                    {
                        ApplyStoredProcedureImplementation(
                            template: template,
                            method: method,
                            storedProcedure: generalizedStoredProcedure,
                            sourceExpressionsByParameterId: parameterIdToSourceExpressions,
                            dbSetPropertiesByModelId: dbSetPropertiesByModelId,
                            applyReturnStatement: true,
                            assignResultToVariable: true,
                            resultExpressionsByModel: out _);

                        return;
                    }
                }, 2);
            }

            var storedProcedureModel = childElement.AsStoredProcedureModel();
            if (storedProcedureModel != null)
            {
                var generalizedStoredProcedure = new GeneralizedStoredProcedure(storedProcedureModel);
                var parameterIdToSourceExpressions = storedProcedureModel.Parameters.ToDictionary(x => x.Id, x => x.Name.ToCamelCase());
                var methodName = storedProcedureModel.InternalElement.Name.ToPascalCase();
                var returnType = GetStoredProcedureReturnType(template, generalizedStoredProcedure);

                @class.AddMethod(returnType, methodName, method =>
                {
                    method.AddMetadata("model", storedProcedureModel);
                    method.RepresentsModel(storedProcedureModel);

                    foreach (var parameter in generalizedStoredProcedure.Parameters.Where(x => x.StoredProcedureDetails.Direction
                                 is StoredProcedureParameterDirection.In
                                 or StoredProcedureParameterDirection.Both))
                    {
                        method.AddParameter(template.GetTypeName(parameter.TypeReference), parameter.InternalElement.Name.ToLocalVariableName());
                    }

                    ApplyStoredProcedureImplementation(
                        template: template,
                        method: method,
                        storedProcedure: generalizedStoredProcedure,
                        sourceExpressionsByParameterId: parameterIdToSourceExpressions,
                        dbSetPropertiesByModelId: dbSetPropertiesByModelId,
                        applyReturnStatement: true,
                        assignResultToVariable: true,
                        out _);

                    method.Async();
                    method.AddOptionalCancellationTokenParameter();

                });
            }
        }
    }

    private static bool TryGetStereotypeStoredProcedure(
        OperationModel operationModel,
        [NotNullWhen(true)] out GeneralizedStoredProcedure? storedProcedure,
        [NotNullWhen(true)] out Dictionary<string, string>? parameterIdToSourceExpressions)
    {
        if (!operationModel.TryGetStoredProcedure(out var stereotype))
        {
            storedProcedure = null;
            parameterIdToSourceExpressions = null;
            return false;
        }

        storedProcedure = new GeneralizedStoredProcedure(operationModel, stereotype);
        parameterIdToSourceExpressions = operationModel.Parameters.ToDictionary(x => x.Id, x => x.Name.ToCamelCase());
        return true;
    }

    private static bool TryGetMappedStoredProcedure(
        OperationModel operationModel,
        ICSharpFileBuilderTemplate template,
        [NotNullWhen(true)] out GeneralizedStoredProcedure? storedProcedure,
        [NotNullWhen(true)] out Dictionary<string, string>? parameterIdToSourceExpressions)
    {
        var invocationTarget = operationModel.StoredProcedureInvocationTargets().FirstOrDefault();
        var storedProcedureModel = invocationTarget?.TypeReference?.Element?.AsStoredProcedureModel();

        if (storedProcedureModel == null)
        {
            storedProcedure = null;
            parameterIdToSourceExpressions = null;
            return false;
        }

        var generalizedStoredProcedure = new GeneralizedStoredProcedure(storedProcedureModel);
        var invocationMapping = invocationTarget.GetMapInvocationMapping();
        var resultMapping = invocationTarget.GetMapInvocationMapping();

        if (invocationMapping == null || resultMapping == null)
        {
            storedProcedure = null;
            parameterIdToSourceExpressions = null;
            return false;
        }

        var inParameters = generalizedStoredProcedure.Parameters
            .Where(x => x.StoredProcedureDetails.Direction is StoredProcedureParameterDirection.In or StoredProcedureParameterDirection.Both)
            .ToArray();
        if (inParameters.Length == 0)
        {
            storedProcedure = generalizedStoredProcedure;
            parameterIdToSourceExpressions = [];
            return true;
        }

        storedProcedure = generalizedStoredProcedure;
        parameterIdToSourceExpressions = new Dictionary<string, string>();

        var mappingManager = new CSharpClassMappingManager(template);
        mappingManager.SetFromReplacement(operationModel, string.Empty);

        foreach (var end in invocationMapping.MappedEnds)
        {
            if (end.SourceElement.Id == operationModel.Id)
            {
                continue;
            }

            var destinationElementId = end.TargetElement.Id;
            var sourceStatement = mappingManager.GenerateSourceStatementForMapping(invocationMapping, end).ToString();

            parameterIdToSourceExpressions.Add(destinationElementId, sourceStatement);
        }

        return true;
    }

    private static void ApplyStoredProcedureImplementation(
        ICSharpFileBuilderTemplate template,
        CSharpClassMethod method,
        GeneralizedStoredProcedure storedProcedure,
        Dictionary<string, string> sourceExpressionsByParameterId,
        Lazy<Dictionary<string, CSharpProperty>> dbSetPropertiesByModelId,
        bool applyReturnStatement,
        bool assignResultToVariable,
        out Dictionary<IMetadataModel, string> resultExpressionsByModel)
    {
        storedProcedure.Validate();

        template.AddTypeSource(TemplateRoles.Domain.Enum);
        template.AddTypeSource(TemplateRoles.Domain.Entity.Interface);
        template.AddTypeSource(TemplateRoles.Domain.DataContract);
        template.CSharpFile.AddUsing("System.Threading");
        template.CSharpFile.AddUsing("System.Threading.Tasks");

        var outputs = storedProcedure.Parameters
            .Where(parameter => parameter.StoredProcedureDetails?.Direction is StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both)
            .Select(parameter =>
            {
                var sqlParameter = parameter.InternalElement.Name.ToCamelCase().EnsureSuffixedWith("Parameter");
                var typeName = template.GetTypeName(parameter.TypeReference, "{0}");

                return new
                {
                    Model = (IMetadataModel)parameter.InternalElement,
                    Expression = $"({typeName}){sqlParameter}.Value",
                };
            })
            .ToList();

        var returnsCollection = storedProcedure.TypeReference.IsCollection;
        var resultVariableName = returnsCollection ? "results" : "result";

        var returnTypeElement = storedProcedure.TypeReference.Element;
        if (returnTypeElement != null)
        {
            outputs.Insert(0, new
            {
                Model = (IMetadataModel)storedProcedure.InternalElement,
                Expression = resultVariableName,
            });
        }

        var returnsScalar = returnTypeElement?.SpecializationType == "Type-Definition";
        if (returnsScalar && returnsCollection)
        {
            throw new ElementException(storedProcedure.InternalElement, "Collection of scalar return types from stored procedures is not supported");
        }

        var spName = !string.IsNullOrWhiteSpace(storedProcedure.Name)
            ? storedProcedure.Name
            : storedProcedure.InternalElement.Name;

        var provider = template.ExecutionContext.Settings.GetDatabaseSettings().DatabaseProvider().AsEnum();

        if (storedProcedure.InternalElement.Package.AsDomainPackageModel().TryGetDatabaseSettings(out var stereotype))
        {
            // Check per Package
            switch (stereotype.DatabaseProvider().AsEnum())
            {
                case Intent.EntityFrameworkCore.Api.DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum.SQLServer:
                    provider = Intent.Modules.EntityFrameworkCore.Settings.DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer;
                    break;
                case Intent.EntityFrameworkCore.Api.DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum.PostgreSQL:
                    provider = Intent.Modules.EntityFrameworkCore.Settings.DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql;
                    break;
                case DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum.MySQL:
                    provider = Intent.Modules.EntityFrameworkCore.Settings.DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql;
                    break;
                case DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum.Oracle:
                    provider = Intent.Modules.EntityFrameworkCore.Settings.DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Oracle;
                    break;
                case DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum.InMemory:
                    provider = Intent.Modules.EntityFrameworkCore.Settings.DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory;
                    break;
                case DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum.Default:
                default:
                    throw new ArgumentOutOfRangeException(nameof(Intent.EntityFrameworkCore.Api.DomainPackageModelStereotypeExtensions.DatabaseSettings.DatabaseProviderOptionsEnum), stereotype.DatabaseProvider().AsEnum(), "Unsupported provider");
            }
        }


        IDbParameterFactory parameterFactory = provider switch
        {
            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.InMemory => expr,
            DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.SqlServer => new SqlDbParameterFactory(template),
            DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Postgresql => new PostgresDbParameterFactory(template),
            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.MySql => expr,
            //DatabaseSettingsExtensions.DatabaseProviderOptionsEnum.Cosmos => expr,
            _ => new DefaultDbParameterFactory(template)
        };

        if (storedProcedure.Parameters.Any(x => x.StoredProcedureDetails != null && x.StoredProcedureDetails.Direction == StoredProcedureParameterDirection.Both))
        {
            method.AddStatement($"throw new {template.UseType("System.NotSupportedException")}(\"" +
                                $"One or more parameters have a direction of both which is not presently supported, " +
                                $"please reach out to us at https://github.com/IntentArchitect/Support should you " +
                                $"need support added.\");");
            resultExpressionsByModel = [];
            return;
        }

        var parameters = new List<(string SpParameterName, string VariableName, string OutputKeyword)>();

        foreach (var parameter in storedProcedure.Parameters)
        {
            if (parameter.StoredProcedureDetails == null)
            {
                continue;
            }

            var isOutputParameter = parameter.StoredProcedureDetails.Direction is
                StoredProcedureParameterDirection.Out or
                StoredProcedureParameterDirection.Both;
            var isUserDefinedTableType = parameter.TypeReference.Element.IsDataContractModel();
            var output = isOutputParameter ? " OUTPUT" : string.Empty;
            var spParameterName = parameter.InternalElement.Name.ToLocalVariableName();
            var sourceExpression = sourceExpressionsByParameterId.GetValueOrDefault(parameter.Id) ?? spParameterName;
            var variableName = isOutputParameter || isUserDefinedTableType || returnsScalar
                ? spParameterName.EnsureSuffixedWith("Parameter")
                : sourceExpression;

            parameters.Add((spParameterName, variableName, output));

            if (isOutputParameter)
            {
                method.AddStatement(parameterFactory.CreateForOutput(
                    invocationPrefix: $"var {variableName} = ",
                    valueVariableName: spParameterName,
                    parameter: parameter));
            }
            else if (returnsScalar)
            {
                method.AddStatement(parameterFactory.CreateForInput(
                    invocationPrefix: $"var {variableName} = ",
                    valueVariableName: spParameterName,
                    parameter: parameter));
            }
            else if (isUserDefinedTableType)
            {
                method.AddStatement(parameterFactory.CreateForTableType(
                    invocationPrefix: $"var {variableName} = ",
                    parameter: parameter));
            }
        }

        if (parameterFactory is DefaultDbParameterFactory)
        {
            method.AddStatement("throw new NotImplementedException();");
            resultExpressionsByModel = outputs.ToDictionary(x => x.Model, x => x.Expression); // Check this
            return;
        }
        else
        {
            if (returnsScalar)
            {
                var sql = $"\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" @{x.SpParameterName}{x.OutputKeyword}"))}\"";

                if (parameterFactory is PostgresDbParameterFactory)
                {
                    sql = $"\"CALL {spName}({string.Join(",", parameters.Select((x, index) => index == 0 ? $"@{x.SpParameterName}" : $" @{x.SpParameterName}"))})\"";
                }

                method.AddInvocationStatement($"var result = await _dbContext.ExecuteScalarAsync<{template.GetTypeName(storedProcedure.TypeReference)}>",
                    s =>
                    {
                        s.AddArgument(sql);

                        foreach (var parameter in parameters)
                        {
                            s.AddArgument($"{parameter.VariableName}");
                        }
                    });

                if (applyReturnStatement)
                {
                    AddReturnStatement(outputs.Select(x => x.Expression), method);
                }
            }
            else if (returnTypeElement == null)
            {
                var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" {{{x.VariableName}}}{x.OutputKeyword}"))}\"";

                if (parameterFactory is PostgresDbParameterFactory)
                {
                    sql = $"$\"CALL {spName}({string.Join(",", parameters.Select((x, index) => index == 0 ? $"{{{x.VariableName}}}" : $" {{{x.VariableName}}}"))})\"";
                }

                template.CSharpFile.AddUsing("Microsoft.EntityFrameworkCore");
                method.AddStatement($"await _dbContext.Database.ExecuteSqlInterpolatedAsync({sql}, cancellationToken);");

                if (applyReturnStatement)
                {
                    AddReturnStatement(outputs.Select(x => x.Expression), method);
                }
            }
            else
            {
                var sql = $"$\"EXECUTE {spName}{string.Join(",", parameters.Select(x => $" {{{x.VariableName}}}{x.OutputKeyword}"))}\"";

                if (parameterFactory is PostgresDbParameterFactory)
                {
                    sql = $"$\"SELECT * FROM {spName}({string.Join(",", parameters.Select((x, index) => index == 0 ? $"{{{x.VariableName}}}" : $" {{{x.VariableName}}}"))})\"";
                }

                template.CSharpFile.AddUsing("Microsoft.EntityFrameworkCore");
                var assignment = assignResultToVariable
                    ? $"var {resultVariableName} = "
                    : string.Empty;
                var (openingBracket, closingBracket) = assignResultToVariable && !returnsCollection
                    ? ("(", ")")
                    : (string.Empty, string.Empty);
                method.AddMethodChainStatement($"{assignment}{openingBracket}await GetSet()", chainStatement =>
                {
                    if (template is not RepositoryTemplate repositoryTemplate ||
                        repositoryTemplate.Model.Id != returnTypeElement.Id)
                    {
                        // If we do this too early the DbSets have not yet been generated:
                        template.CSharpFile.AfterBuild(_ =>
                        {
                            chainStatement.Text = chainStatement.Text.Replace("GetSet()", $"_dbContext.{dbSetPropertiesByModelId.Value[returnTypeElement.Id].Name}");
                        }, 1000);
                    }

                    template.CSharpFile.AddUsing("Microsoft.EntityFrameworkCore");
                    chainStatement
                        .AddChainStatement($"FromSqlInterpolated({sql})")
                        .AddChainStatement("IgnoreQueryFilters()")
                        .AddChainStatement($"ToArrayAsync(cancellationToken){closingBracket}");

                    if (!returnsCollection)
                    {
                        template.CSharpFile.AddUsing("System.Linq");
                        chainStatement.AddChainStatement(storedProcedure.TypeReference.IsNullable
                            ? "SingleOrDefault()"
                            : "Single()");
                    }

                    if (applyReturnStatement)
                    {
                        AddReturnStatement(outputs.Select(x => x.Expression), method);
                    }
                });
            }
        }

        resultExpressionsByModel = outputs.ToDictionary(x => x.Model, x => x.Expression);
    }

    private static void AddReturnStatement(IEnumerable<string> returnTupleProperties, CSharpClassMethod method)
    {
        var asArray = returnTupleProperties.ToArray();

        switch (asArray.Length)
        {
            case 0:
                return;
            case 1:
                method.AddStatement($"return {asArray[0]};", s => s.SeparatedFromPrevious());
                return;
            case > 1:
                method.AddStatement($"return ({string.Join(", ", asArray)});", s => s.SeparatedFromPrevious());
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static IReadOnlyCollection<StoredProcedureModel> GetStoredProcedureModels(this RepositoryModel repositoryModel)
    {
        return repositoryModel.InternalElement.ChildElements
            .SelectMany(childElement =>
            {
                var storedProcedureModel = childElement.AsStoredProcedureModel();
                if (storedProcedureModel != null)
                {
                    return [storedProcedureModel];
                }

                var operationModel = OperationModelExtensions.AsOperationModel(childElement);
                if (operationModel != null)
                {
                    return operationModel.StoredProcedureInvocationTargets()
                        .Select(x => x.TypeReference?.Element?.AsStoredProcedureModel()!)
                        .Where(x => x != null);
                }

                return [];
            })
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

    private static List<TupleEntry> GetReturnProperties(ICSharpTemplate template, GeneralizedStoredProcedure storedProcedure)
    {
        var tupleProperties = storedProcedure.Parameters
            .Where(parameter => parameter.StoredProcedureDetails?.Direction is StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both)
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

    private static CSharpType GetStoredProcedureReturnType(ICSharpTemplate template, GeneralizedStoredProcedure storedProcedure)
    {
        var tupleProperties = GetReturnProperties(template, storedProcedure);

        return tupleProperties.Count switch
        {
            0 => CSharpType.CreateTask(template),
            1 => CSharpType.CreateTask(new CSharpTypeName(tupleProperties[0].TypeName), template),
            _ => CSharpType.CreateTask(new CSharpTypeTuple(tupleProperties.Select(s => new CSharpTupleElement(new CSharpTypeName(s.TypeName), s.Name)).ToList()), template)
        };
    }

    private class MappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpTemplate _sourceTemplate;

        public MappingResolver(ICSharpTemplate sourceTemplate)
        {
            _sourceTemplate = sourceTemplate;
        }

        public ICSharpMapping ResolveMappings(MappingModel mappingModel)
        {
            if (mappingModel.Mapping?.SourceElement?.TypeReference?.IsCollection == true)
            {
                return new SelectToListMapping(mappingModel, _sourceTemplate);
            }

            if (mappingModel.Model.IsDataContractModel())
            {
                return new ConstructorMapping(mappingModel, _sourceTemplate);
            }

            return new ObjectInitializationMapping(mappingModel, _sourceTemplate);
        }
    }

    private record StaticMetadata(string Id) : IMetadataModel;
}