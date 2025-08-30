using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Aws.Lambda.Functions.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Aws.Lambda.Functions.Dispatch.MediatR.Templates.Endpoints;

public class CqrsLambdaFunctionModel : ILambdaFunctionModel
{
    public CqrsLambdaFunctionModel(ILambdaFunctionContainerModel container, IElement endpointElement, ISoftwareFactoryExecutionContext context)
    {
        if (!context.TryGetHttpEndpoint(
                element: endpointElement,
                defaultBasePath: null,
                httpEndpointModel: out var httpEndpoint))
        {
            throw new InvalidOperationException("Could not obtain endpoint model");
        }

        Id = endpointElement.Id;
        Name = httpEndpoint.Name;
        InternalElement = endpointElement;
        Container = container;
        Comment = endpointElement.Comment;
        TypeReference = endpointElement.TypeReference;
        Verb = httpEndpoint.Verb;
        Route = $"{httpEndpoint.BaseRoute}/{httpEndpoint.SubRoute}";
        MediaType = httpEndpoint.MediaType;
        Parameters = httpEndpoint.Inputs.Select(GetInput).ToList();
        ReturnType = endpointElement.TypeReference.Element != null ? endpointElement.TypeReference : null;
        Stereotypes = endpointElement.Stereotypes;
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
        return new EndpointParameterModel(model);
    }
}