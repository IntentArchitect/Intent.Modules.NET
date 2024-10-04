using System.Collections.Generic;
using Intent.EntityFrameworkCore.Repositories.Api;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Domain.Repositories.Api;
using Intent.Modules.Common;
using Intent.Modules.EntityFrameworkCore.Repositories.Api;
using Intent.Modules.Modelers.Domain.StoredProcedures.Api;

namespace Intent.Modules.EntityFrameworkCore.Repositories.Templates;

internal static class RepositoryModelExtensions
{
    public static IReadOnlyCollection<GeneralizedStoredProcedure> GetGeneralizedStoredProcedures(this RepositoryModel repositoryModel)
    {
        var results = new List<GeneralizedStoredProcedure>();

        foreach (var childElement in repositoryModel.InternalElement.ChildElements)
        {
            var operationModel = childElement.AsOperationModel();
            if (operationModel != null && operationModel.TryGetStoredProcedure(out var storedProcedure))
            {
                results.Add(new GeneralizedStoredProcedure(operationModel, storedProcedure));
            }

            if (childElement.IsStoredProcedureModel())
            {
                results.Add(new GeneralizedStoredProcedure(childElement.AsStoredProcedureModel()));
            }
        }

        return results;
    }
}

internal class GeneralizedStoredProcedure : IElementWrapper, IHasTypeReference, IMetadataModel
{
    public GeneralizedStoredProcedure(OperationModel model, StoredProcedureStereotype storedProcedureStereotype)
    {
        Name = storedProcedureStereotype.GetName();
        Model = model;
        InternalElement = model.InternalElement;

        foreach (var parameterModel in model.Parameters)
        {
            var parameter = new Parameter
            {
                Model = parameterModel,
                InternalElement = parameterModel.InternalElement
            };

            if (parameterModel.TryGetStoredProcedureParameter(out var storedProcedureParameter))
            {
                parameter.StoredProcedureDetails = new ParameterStoredProcedureDetails
                {
                    Name = storedProcedureParameter.GetName(),
                    Direction = storedProcedureParameter.GetDirection(),
                    Size = storedProcedureParameter.GetSize(),
                    Precision = storedProcedureParameter.GetPrecision(),
                    Scale = storedProcedureParameter.GetScale(),
                    SqlStringType = storedProcedureParameter.GetSqlStringType()
                };
            }

            Parameters.Add(parameter);
        }
    }

    public GeneralizedStoredProcedure(StoredProcedureModel model)
    {
        var storedProcedure = model.GetStoredProcedureSettings();

        Name = storedProcedure.NameInSchema();
        Model = model;
        InternalElement = model.InternalElement;

        foreach (var parameterModel in model.Parameters)
        {
            var storedProcedureParameter = parameterModel.GetStoredProcedureParameterSettings();

            Parameters.Add(new Parameter
            {
                Model = parameterModel,
                InternalElement = parameterModel.InternalElement,
                StoredProcedureDetails = new ParameterStoredProcedureDetails
                {
                    Name = parameterModel.Name,
                    Direction = !storedProcedureParameter.IsOutputParameter()
                        ? StoredProcedureParameterDirection.In
                        : StoredProcedureParameterDirection.Out,
                    Size = storedProcedureParameter.Size(),
                    Precision = storedProcedureParameter.Precision(),
                    Scale = storedProcedureParameter.Scale(),
                    SqlStringType = storedProcedureParameter.SQLStringType().Value
                }
            });
        }
    }

    /// <summary>
    /// The name in the schema.
    /// </summary>
    public string Name { get; set; }

    public List<Parameter> Parameters { get; set; } = new();
    public IMetadataModel Model { get; set; }
    public IElement InternalElement { get; }
    public ITypeReference TypeReference => InternalElement.TypeReference;
    public string Id => InternalElement.Id;
}

internal class Parameter : IElementWrapper, IHasTypeReference, IMetadataModel
{
    public Parameter() { }

    public IMetadataModel Model { get; set; }
    public IElement InternalElement { get; set; }
    public ITypeReference TypeReference => InternalElement.TypeReference;
    public string Id => InternalElement.Id;
    public ParameterStoredProcedureDetails StoredProcedureDetails { get; set; }
}

internal class ParameterStoredProcedureDetails
{
    /// <summary>
    /// The name in the schema.
    /// </summary>
    public string Name { get; set; }
    public StoredProcedureParameterDirection Direction { get; set; }
    public int? Size { get; set; }
    public int? Precision { get; set; }
    public int? Scale { get; set; }
    public string SqlStringType { get; set; }
}
