#nullable enable
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;
using Intent.Utils;
using OperationModel = Intent.Modelers.Domain.Api.OperationModel;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

namespace Intent.Modules.Dapper.Templates.StoredProcedures;

internal static class RepositoryStoredProcedureExtensions
{
    /// <summary>
    /// The <c>Stored Procedure</c> elements modelled directly under the <paramref name="repositoryModel"/>.
    /// Each of these gets its own repository method.
    /// </summary>
    public static IReadOnlyCollection<GeneralizedStoredProcedure> GetStoredProcedureElements(this RepositoryModel repositoryModel)
    {
        return repositoryModel.InternalElement.ChildElements
            .Where(childElement => childElement.IsStoredProcedureModel())
            .Select(childElement => new GeneralizedStoredProcedure(childElement.AsStoredProcedureModel()))
            .ToArray();
    }

    /// <summary>
    /// Whether the repository has anything stored-procedure related on it at all, whether that be a
    /// <c>Stored Procedure</c> element or an <c>Operation</c> which is backed by a stored procedure.
    /// </summary>
    public static bool HasStoredProcedures(this RepositoryModel repositoryModel)
    {
        return repositoryModel.InternalElement.ChildElements.Any(x => x.IsStoredProcedureModel()) ||
            repositoryModel.Operations.Any(x => x.IsStoredProcedureBacked());
    }

    /// <summary>
    /// An <c>Operation</c> is backed by a stored procedure either through the <c>[Stored Procedure]</c>
    /// stereotype, or through a Stored Procedure Invocation association onto a <c>Stored Procedure</c> element.
    /// </summary>
    public static bool IsStoredProcedureBacked(this OperationModel operationModel)
    {
        return operationModel.TryGetStoredProcedure(out _) ||
            operationModel.InvokesStoredProcedureElement();
    }

    public static bool InvokesStoredProcedureElement(this OperationModel operationModel)
    {
        return operationModel.StoredProcedureInvocationTargets()
            .Any(x => x.TypeReference?.Element?.IsStoredProcedureModel() == true);
    }

    /// <summary>
    /// A repository method is only asynchronous when the model says so: either the name ends in
    /// <c>Async</c> or the <c>[Asynchronous]</c> stereotype is applied. This is the same convention the
    /// other repository modules use (see <c>Intent.Modules.Entities.Repositories.Api</c>).
    /// </summary>
    public static bool IsAsync(this OperationModel operationModel)
    {
        return operationModel.Name.EndsWith("Async") || operationModel.HasStereotype("Asynchronous");
    }

    /// <summary>
    /// As per <see cref="IsAsync(OperationModel)"/>, except that the <c>[Asynchronous]</c> stereotype is
    /// only applicable to <c>Operation</c> elements, so in practice a <c>Stored Procedure</c> element can
    /// only be made asynchronous by naming it accordingly.
    /// </summary>
    public static bool IsAsync(this GeneralizedStoredProcedure storedProcedure)
    {
        return storedProcedure.InternalElement.Name.EndsWith("Async") ||
            storedProcedure.InternalElement.HasStereotype("Asynchronous");
    }
}

internal class GeneralizedStoredProcedure : IElementWrapper, IHasTypeReference, IMetadataModel
{
    /// <summary>
    /// An <c>Operation</c> with the <c>[Stored Procedure]</c> stereotype: the operation's own parameters
    /// are the procedure's arguments, in order.
    /// </summary>
    public GeneralizedStoredProcedure(OperationModel model, StoredProcedureStereotype storedProcedureStereotype)
    {
        Name = storedProcedureStereotype.GetName();
        Model = model;
        InternalElement = model.InternalElement;

        foreach (var parameterModel in model.Parameters)
        {
            var schemaName = parameterModel.Name;
            var direction = StoredProcedureParameterDirection.In;

            if (parameterModel.TryGetStoredProcedureParameter(out var storedProcedureParameter))
            {
                schemaName = !string.IsNullOrWhiteSpace(storedProcedureParameter.GetName())
                    ? storedProcedureParameter.GetName()
                    : parameterModel.Name;
                direction = storedProcedureParameter.GetDirection();
            }

            Parameters.Add(new GeneralizedStoredProcedureParameter
            {
                Model = parameterModel,
                InternalElement = parameterModel.InternalElement,
                SchemaName = schemaName,
                Direction = direction
            });
        }
    }

    /// <summary>
    /// A <c>Stored Procedure</c> element. The Dapper module does not read the sizing / output-parameter
    /// stereotypes owned by the Entity Framework Core Repositories module, so the element's own name is
    /// used as the name in the schema and every parameter is treated as an input parameter.
    /// </summary>
    public GeneralizedStoredProcedure(StoredProcedureModel model)
    {
        Name = model.Name;
        Model = model;
        InternalElement = model.InternalElement;

        foreach (var parameterModel in model.Parameters)
        {
            Parameters.Add(new GeneralizedStoredProcedureParameter
            {
                Model = parameterModel,
                InternalElement = parameterModel.InternalElement,
                SchemaName = parameterModel.Name,
                Direction = StoredProcedureParameterDirection.In
            });
        }
    }

    /// <summary>
    /// The name in the schema.
    /// </summary>
    public string? Name { get; }

    public List<GeneralizedStoredProcedureParameter> Parameters { get; } = new();
    public IMetadataModel Model { get; }
    public IElement InternalElement { get; }
    public ITypeReference TypeReference => InternalElement.TypeReference;
    public string Id => InternalElement.Id;

    public string SchemaName => !string.IsNullOrWhiteSpace(Name) ? Name! : InternalElement.Name;

    /// <summary>
    /// Fails the Software Factory run for anything modelled which the Dapper module cannot generate.
    /// </summary>
    public void Validate()
    {
        foreach (var parameter in Parameters)
        {
            if (parameter.Direction is StoredProcedureParameterDirection.Out or StoredProcedureParameterDirection.Both)
            {
                Logging.Log.Failure($"Parameter \"{parameter.InternalElement.Name}\" [{parameter.Id}] on Stored Procedure " +
                    $"\"{InternalElement.Name}\" [{Id}] has a direction of \"{parameter.Direction}\", output " +
                    $"parameters are not supported by the Dapper module.");
            }

            if (parameter.TypeReference.Element.IsDataContractModel())
            {
                Logging.Log.Failure($"Parameter \"{parameter.InternalElement.Name}\" [{parameter.Id}] on Stored Procedure " +
                    $"\"{InternalElement.Name}\" [{Id}] is of type \"Data Contract\", user-defined table type " +
                    $"parameters are not supported by the Dapper module.");
                continue;
            }

            if (parameter.TypeReference.IsCollection)
            {
                Logging.Log.Failure($"Parameter \"{parameter.InternalElement.Name}\" [{parameter.Id}] on Stored Procedure " +
                    $"\"{InternalElement.Name}\" [{Id}] has \"Is Collection\" enabled and is not of type " +
                    $"\"Data Contract\", this is unsupported.");
            }
        }
    }
}

internal class GeneralizedStoredProcedureParameter : IElementWrapper, IHasTypeReference, IMetadataModel
{
    public IMetadataModel Model { get; init; } = null!;
    public IElement InternalElement { get; init; } = null!;
    public ITypeReference TypeReference => InternalElement.TypeReference;
    public string Id => InternalElement.Id;

    /// <summary>
    /// The parameter's name in the schema, i.e. the <c>@placeholder</c> in the generated <c>EXEC</c> statement.
    /// </summary>
    public string SchemaName { get; init; } = null!;

    public StoredProcedureParameterDirection Direction { get; init; }
}
