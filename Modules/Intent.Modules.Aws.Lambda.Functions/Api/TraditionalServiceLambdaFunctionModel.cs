using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.Aws.Lambda.Functions.Api;

public class TraditionalServiceLambdaFunctionModel : ILambdaFunctionModel
{
    public TraditionalServiceLambdaFunctionModel(ILambdaFunctionContainerModel container, OperationModel operationModel, ISoftwareFactoryExecutionContext context)
    {
        if (!context.TryGetHttpEndpoint(
                element: operationModel.InternalElement,
                defaultBasePath: null,
                httpEndpointModel: out var httpEndpoint))
        {
            throw new InvalidOperationException("Could not obtain endpoint model");
        }
        
        Id = operationModel.Id;
        Name = httpEndpoint.Name;
        InternalElement = operationModel.InternalElement;
        Container = container;
        Comment = operationModel.Comment;
        TypeReference = operationModel.TypeReference;
        Verb = httpEndpoint.Verb;
        Route = $"{httpEndpoint.BaseRoute}/{httpEndpoint.SubRoute}";
        MediaType = httpEndpoint.MediaType;
        Parameters = httpEndpoint.Inputs.Select(GetInput).ToList();
        ReturnType = operationModel.ReturnType;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public IEnumerable<IStereotype> Stereotypes { get; }
    public string Comment { get; }
    public ITypeReference? ReturnType { get; }
    public HttpVerb Verb { get; }
    public string? Route { get; }
    public HttpMediaType? MediaType { get; }
    public IElement InternalElement { get; }
    public ILambdaFunctionContainerModel Container { get; }
    public IList<IEndpointParameterModel> Parameters { get; }
    
    private static IEndpointParameterModel GetInput(IHttpEndpointInputModel model)
    {
        return new TraditionalServiceEndpointParameterModel(
            id: model.Id,
            name: model.Name,
            typeReference: model.TypeReference,
            source: model.Source,
            headerName: model.HeaderName,
            queryStringName: model.QueryStringName,
            mappedPayloadProperty: model.MappedPayloadProperty,
            value: model.Value);
    }
}