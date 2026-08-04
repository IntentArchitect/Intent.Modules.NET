#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Mapping;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.Utils;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;

namespace Intent.Modules.Dapper.Templates.StoredProcedures;

/// <summary>
/// Generates the body which invokes a stored procedure through Dapper. The statement shape is chosen
/// from the procedure's return type:
/// <list type="bullet">
/// <item>no return type: <c>Execute</c></item>
/// <item>a <c>Type-Definition</c> (scalar): <c>ExecuteScalar&lt;T&gt;</c></item>
/// <item>a collection: <c>Query&lt;T&gt;</c></item>
/// <item>a single entity / data contract: <c>QuerySingle&lt;T&gt;</c> / <c>QuerySingleOrDefault&lt;T&gt;</c></item>
/// </list>
/// The <c>...Async</c> overload of whichever of the above applies is used when the repository method
/// itself is asynchronous, i.e. when it is named <c>...Async</c> or has the <c>[Asynchronous]</c>
/// stereotype applied.
/// </summary>
internal static class StoredProcedureInvocationHelper
{
    /// <summary>
    /// The id of the static "result" element which a Stored Procedure Invocation's "Map Result"
    /// mapping maps from.
    /// </summary>
    private const string ResultMappableElementId = "1eba9280-3bf0-46f8-981c-414dee8e35c3";

    /// <summary>
    /// Implements an <c>Operation</c> which is backed by a stored procedure, whether through the
    /// <c>[Stored Procedure]</c> stereotype or through a Stored Procedure Invocation association.
    /// </summary>
    public static void ApplyOperationImplementation(
        ICSharpFileBuilderTemplate template,
        CSharpClassMethod method,
        OperationModel operationModel,
        bool isAsync)
    {
        if (TryGetMappedStoredProcedure(template, operationModel, out var mappedStoredProcedure, out var mappedSourceExpressions))
        {
            var resultVariableName = ApplyInvocation(template, method, mappedStoredProcedure, mappedSourceExpressions, isAsync);

            if (operationModel.ReturnType == null)
            {
                return;
            }

            if (resultVariableName == null)
            {
                Logging.Log.Failure($"Operation \"{operationModel.Name}\" [{operationModel.Id}] has a return type but the " +
                    $"Stored Procedure \"{mappedStoredProcedure.InternalElement.Name}\" " +
                    $"[{mappedStoredProcedure.Id}] it invokes does not return anything.");
                return;
            }

            var invocationTarget = operationModel.StoredProcedureInvocationTargets()
                .First(x => x.TypeReference?.Element?.IsStoredProcedureModel() == true);
            var resultMapping = invocationTarget.GetMapResultMapping();

            if (resultMapping == null || mappedStoredProcedure.TypeReference.Equals(operationModel.TypeReference))
            {
                method.AddStatement($"return {resultVariableName};", s => s.SeparatedFromPrevious());
                return;
            }

            var mappingManager = new CSharpClassMappingManager(template);
            mappingManager.AddMappingResolver(new StoredProcedureMappingResolver(template));
            mappingManager.SetFromReplacement(new StaticMetadata(ResultMappableElementId), resultVariableName);

            method.AddStatement(
                new CSharpReturnStatement(mappingManager.GenerateCreationStatement(resultMapping)),
                s => s.SeparatedFromPrevious());

            return;
        }

        if (operationModel.TryGetStoredProcedure(out var stereotype))
        {
            var storedProcedure = new GeneralizedStoredProcedure(operationModel, stereotype);
            var sourceExpressionsByParameterId = operationModel.Parameters
                .ToDictionary(x => x.Id, x => x.Name.ToCamelCase());

            ApplyInvocationWithReturnStatement(template, method, storedProcedure, sourceExpressionsByParameterId, isAsync);
        }
    }

    /// <summary>
    /// Implements the method generated for a <c>Stored Procedure</c> element modelled under a
    /// <c>Repository</c>. Its parameters map one-to-one onto the generated method's parameters.
    /// </summary>
    public static void ApplyStoredProcedureElementImplementation(
        ICSharpFileBuilderTemplate template,
        CSharpClassMethod method,
        GeneralizedStoredProcedure storedProcedure,
        bool isAsync)
    {
        var sourceExpressionsByParameterId = storedProcedure.Parameters
            .ToDictionary(x => x.Id, x => x.InternalElement.Name.ToLocalVariableName());

        ApplyInvocationWithReturnStatement(template, method, storedProcedure, sourceExpressionsByParameterId, isAsync);
    }

    private static void ApplyInvocationWithReturnStatement(
        ICSharpFileBuilderTemplate template,
        CSharpClassMethod method,
        GeneralizedStoredProcedure storedProcedure,
        IReadOnlyDictionary<string, string> sourceExpressionsByParameterId,
        bool isAsync)
    {
        var resultVariableName = ApplyInvocation(template, method, storedProcedure, sourceExpressionsByParameterId, isAsync);
        if (resultVariableName != null)
        {
            method.AddStatement($"return {resultVariableName};", s => s.SeparatedFromPrevious());
        }
    }

    /// <summary>
    /// Adds the connection, SQL, parameter object and Dapper invocation statements to
    /// <paramref name="method"/> and returns the name of the variable the result was assigned to
    /// (<c>null</c> when the procedure returns nothing).
    /// </summary>
    private static string? ApplyInvocation(
        ICSharpFileBuilderTemplate template,
        CSharpClassMethod method,
        GeneralizedStoredProcedure storedProcedure,
        IReadOnlyDictionary<string, string> sourceExpressionsByParameterId,
        bool isAsync)
    {
        storedProcedure.Validate();

        template.CSharpFile
            .AddUsing("System.Data")
            .AddUsing("System.Linq")
            .AddUsing("Dapper");

        if (isAsync)
        {
            template.CSharpFile
                .AddUsing("System.Threading")
                .AddUsing("System.Threading.Tasks");
        }

        var parameters = GetSqlParameters(storedProcedure, sourceExpressionsByParameterId);

        var sql = $"EXEC {storedProcedure.SchemaName}";
        if (parameters.Count > 0)
        {
            sql += $" {string.Join(", ", parameters.Select(x => $"@{x.Name}"))}";
        }

        method.AddStatement("using var connection = GetConnection();");
        method.AddStatement($"var sql = \"{sql}\";");

        var commandArguments = "sql";
        if (parameters.Count > 0)
        {
            method.AddStatement($"var parameters = new {{ {string.Join(", ", parameters.Select(GetParameterInitializer))} }};");
            commandArguments = "sql, parameters";
        }

        // Dapper only accepts a CancellationToken through a CommandDefinition, and only its async
        // overloads take one, so the synchronous calls pass the arguments directly:
        var command = isAsync
            ? $"new {template.UseType("Dapper.CommandDefinition")}({commandArguments}, cancellationToken: cancellationToken)"
            : commandArguments;
        var awaitPrefix = isAsync ? "await " : string.Empty;
        var asyncSuffix = isAsync ? "Async" : string.Empty;

        var typeReference = storedProcedure.TypeReference;
        var returnTypeElement = typeReference?.Element;

        if (returnTypeElement == null)
        {
            method.AddStatement($"{awaitPrefix}connection.Execute{asyncSuffix}({command});", s => s.SeparatedFromPrevious());
            return null;
        }

        var returnTypeName = template.GetTypeName(typeReference!, "{0}");

        if (returnTypeElement.SpecializationType == "Type-Definition")
        {
            if (typeReference!.IsCollection)
            {
                Logging.Log.Failure($"Stored Procedure \"{storedProcedure.InternalElement.Name}\" [{storedProcedure.Id}] returns a " +
                    $"collection of the scalar type \"{returnTypeElement.Name}\", this is unsupported.");
            }

            method.AddStatement($"var result = {awaitPrefix}connection.ExecuteScalar{asyncSuffix}<{returnTypeName}>({command});", s => s.SeparatedFromPrevious());
            return "result";
        }

        if (typeReference!.IsCollection)
        {
            // The async overload returns an IEnumerable<T> inside a Task, so it needs bracketing before
            // ToList() can be called on it:
            var query = $"{awaitPrefix}connection.Query{asyncSuffix}<{returnTypeName}>({command})";
            method.AddStatement($"var results = {(isAsync ? $"({query})" : query)}.ToList();", s => s.SeparatedFromPrevious());
            return "results";
        }

        var querySingle = typeReference.IsNullable ? "QuerySingleOrDefault" : "QuerySingle";
        method.AddStatement($"var result = {awaitPrefix}connection.{querySingle}{asyncSuffix}<{returnTypeName}>({command});", s => s.SeparatedFromPrevious());
        return "result";
    }

    /// <summary>
    /// An anonymous type infers its member name from the expression when that expression is just an
    /// identifier, so <c>new { StartDate }</c> is the same as <c>new { StartDate = StartDate }</c>. The
    /// explicit form is only emitted when the SQL parameter name and the source expression differ.
    /// </summary>
    private static string GetParameterInitializer(SqlParameter parameter)
    {
        return parameter.Name == parameter.SourceExpression
            ? parameter.Name
            : $"{parameter.Name} = {parameter.SourceExpression}";
    }

    private static List<SqlParameter> GetSqlParameters(
        GeneralizedStoredProcedure storedProcedure,
        IReadOnlyDictionary<string, string> sourceExpressionsByParameterId)
    {
        var results = new List<SqlParameter>();
        var usedNames = new HashSet<string>();

        foreach (var parameter in storedProcedure.Parameters.Where(x => x.Direction == StoredProcedureParameterDirection.In))
        {
            var baseName = (!string.IsNullOrWhiteSpace(parameter.SchemaName)
                ? parameter.SchemaName
                : parameter.InternalElement.Name).ToPascalCase();

            var name = baseName;
            var suffix = 1;
            while (!usedNames.Add(name))
            {
                name = $"{baseName}{++suffix}";
            }

            var sourceExpression = sourceExpressionsByParameterId.TryGetValue(parameter.Id, out var expression) && !string.IsNullOrWhiteSpace(expression)
                ? expression
                : parameter.InternalElement.Name.ToLocalVariableName();

            results.Add(new SqlParameter(name, sourceExpression));
        }

        return results;
    }

    private static bool TryGetMappedStoredProcedure(
        ICSharpFileBuilderTemplate template,
        OperationModel operationModel,
        [NotNullWhen(true)] out GeneralizedStoredProcedure? storedProcedure,
        [NotNullWhen(true)] out Dictionary<string, string>? sourceExpressionsByParameterId)
    {
        var invocationTarget = operationModel.StoredProcedureInvocationTargets()
            .FirstOrDefault(x => x.TypeReference?.Element?.IsStoredProcedureModel() == true);
        var storedProcedureModel = invocationTarget?.TypeReference?.Element?.AsStoredProcedureModel();

        if (storedProcedureModel == null)
        {
            storedProcedure = null;
            sourceExpressionsByParameterId = null;
            return false;
        }

        storedProcedure = new GeneralizedStoredProcedure(storedProcedureModel);
        sourceExpressionsByParameterId = new Dictionary<string, string>();

        var invocationMapping = invocationTarget!.GetMapInvocationMapping();
        if (invocationMapping == null)
        {
            return true;
        }

        var mappingManager = new CSharpClassMappingManager(template);
        mappingManager.SetFromReplacement(operationModel, string.Empty);

        foreach (var end in invocationMapping.MappedEnds)
        {
            if (end.SourceElement.Id == operationModel.Id)
            {
                continue;
            }

            sourceExpressionsByParameterId[end.TargetElement.Id] = mappingManager
                .GenerateSourceStatementForMapping(invocationMapping, end)
                .ToString();
        }

        return true;
    }

    private record SqlParameter(string Name, string SourceExpression);

    private record StaticMetadata(string Id) : IMetadataModel;

    private class StoredProcedureMappingResolver : IMappingTypeResolver
    {
        private readonly ICSharpTemplate _sourceTemplate;

        public StoredProcedureMappingResolver(ICSharpTemplate sourceTemplate)
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
}
