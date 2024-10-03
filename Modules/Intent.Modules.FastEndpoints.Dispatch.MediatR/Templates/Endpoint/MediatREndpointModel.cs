using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Dispatch.MediatR.Templates.Endpoint;

public class MediatREndpointContainerModel : IEndpointContainerModel
{
    public MediatREndpointContainerModel(
        IElement parentElement,
        IEnumerable<IElement> elements)
    {
        Id = parentElement?.Id ?? Guid.Empty.ToString();
        Name = parentElement is not null
            ? string.Join(string.Empty,
                parentElement.GetParentPath().Concat(new[] { parentElement })
                    .Select(s => s.Name?.Replace(".", "_").ToPascalCase() ?? string.Empty))
            : "Default";
        Folder = parentElement?.ParentElement?.AsFolderModel();
        Endpoints = elements.Select(IEndpointModel (operation) => new MediatREndpointModel(this, operation)).ToList();
    }

    public string Id { get; }
    public FolderModel Folder { get; }
    public string Name { get; }
    public IElement InternalElement { get; }
    public IList<IEndpointModel> Endpoints { get; }
}

public class MediatREndpointModel : IEndpointModel
{
    public MediatREndpointModel(
        MediatREndpointContainerModel containerModel,
        IElement endpoint)
    {
        var httpEndpoint = HttpEndpointModelFactory.GetEndpoint(endpoint)!;

        Id = endpoint.Id;
        Comment = endpoint.Comment;
        Name = endpoint.Name;
        TypeReference = endpoint.TypeReference;
        Verb = httpEndpoint.Verb;
        Route = httpEndpoint.Route;
        MediaType = httpEndpoint.MediaType;
        InternalElement = endpoint;
        Container = containerModel;
        Parameters = httpEndpoint.Inputs.Select(GetInput).ToList();
    }

    public string Id { get; }
    public string Comment { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public ITypeReference? ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string Route { get; }
    public HttpMediaType? MediaType { get; }
    public IElement InternalElement { get; }
    public IEndpointContainerModel Container { get; }
    public IList<IEndpointParameterModel> Parameters { get; }
    
    private static IEndpointParameterModel GetInput(IHttpEndpointInputModel model)
    {
        return new MediatREndpointParameterModel(
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

public class MediatREndpointParameterModel : IEndpointParameterModel
{
    public MediatREndpointParameterModel(
        string id,
        string name,
        ITypeReference typeReference,
        HttpInputSource? source,
        string? headerName,
        string? queryStringName,
        ICanBeReferencedType? mappedPayloadProperty,
        string? value)
    {
        Id = id;
        Name = name;
        TypeReference = typeReference;
        Source = source;
        HeaderName = headerName;
        QueryStringName = queryStringName;
        MappedPayloadProperty = mappedPayloadProperty;
        Value = value;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public HttpInputSource? Source { get; }
    public string? HeaderName { get; }
    public string? QueryStringName { get; }
    public ICanBeReferencedType? MappedPayloadProperty { get; }
    public string? Value { get; }
}