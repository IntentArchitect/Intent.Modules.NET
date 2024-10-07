using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class ServiceEndpointModel : IEndpointModel
{
    private readonly ServiceModel _serviceModel;
    private readonly OperationModel _operationModel;

    public ServiceEndpointModel(IEndpointContainerModel container, ServiceModel serviceModel, OperationModel operationModel, IAuthorizationModel? authorizationModel)
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
        
        _serviceModel = serviceModel;
        _operationModel = operationModel;

        var httpEndpoint = HttpEndpointModelFactory.GetEndpoint(operationModel.InternalElement)!;

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
        Authorization = authorizationModel;
    }

    public string Id { get; }
    public string Name { get; }
    public IElement InternalElement { get; }
    public IEndpointContainerModel Container { get; }
    public string Comment { get; }
    public ITypeReference TypeReference { get; }
    public ITypeReference? ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string Route { get; }
    public HttpMediaType? MediaType { get; }
    public IList<IEndpointParameterModel> Parameters { get; }
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel? Authorization { get; }
    public FolderModel? Folder => new FolderModel(Container.InternalElement, Container.InternalElement.SpecializationType);

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