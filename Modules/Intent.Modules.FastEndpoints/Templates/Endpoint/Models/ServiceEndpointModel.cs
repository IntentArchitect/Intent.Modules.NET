using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class ServiceEndpointModel : IEndpointModel
{
    public ServiceEndpointModel(
        IEndpointContainerModel container, 
        ServiceModel serviceModel, 
        OperationModel operationModel, 
        ISoftwareFactoryExecutionContext context,
        IReadOnlyCollection<ISecurityModel> securityModels)
    {
        if (container is null)
        {
            ArgumentNullException.ThrowIfNull(container);
        }
        if (serviceModel is null)
        {
            ArgumentNullException.ThrowIfNull(serviceModel);
        }
        if (operationModel is null)
        {
            ArgumentNullException.ThrowIfNull(operationModel);
        }

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
        Route = httpEndpoint.Route;
        MediaType = httpEndpoint.MediaType;
        Parameters = httpEndpoint.Inputs.Select(GetInput).ToList();
        RequiresAuthorization = httpEndpoint.RequiresAuthorization;
        AllowAnonymous = httpEndpoint.AllowAnonymous;
        SecurityModels = securityModels;
        ApplicableVersions = operationModel.GetApiVersionSettings()
            ?.ApplicableVersions()
            .Select(s => new EndpointApiVersionModel(s))
            .Cast<IApiVersionModel>()
            .ToArray() ?? [];
    }

    public string Id { get; }
    public string Name { get; }
    public IElement InternalElement { get; }
    public IEndpointContainerModel Container { get; }
    public string Comment { get; }
    public ITypeReference TypeReference { get; }
    public ITypeReference? ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string? Route { get; }
    public HttpMediaType? MediaType { get; }
    public IList<IEndpointParameterModel> Parameters { get; }
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public IReadOnlyCollection<IApiVersionModel> ApplicableVersions { get; }
    public FolderModel? Folder => Container.InternalElement != null
        ? new FolderModel(Container.InternalElement, Container.InternalElement.SpecializationType)
        : null;

    private static IEndpointParameterModel GetInput(IHttpEndpointInputModel model)
    {
        return new ServiceEndpointParameterModel(
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