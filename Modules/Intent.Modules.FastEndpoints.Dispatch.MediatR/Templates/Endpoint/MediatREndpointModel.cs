using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.Endpoint.Models;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Dispatch.MediatR.Templates.Endpoint;

public class MediatREndpointContainerModel : IEndpointContainerModel
{
    public MediatREndpointContainerModel(
        IElement? parentElement,
        IEnumerable<IElement> elements,
        bool securedByDefault)
    {
        Id = parentElement?.Id ?? Guid.Empty.ToString();
        Name = parentElement is not null
            ? string.Join(string.Empty,
                parentElement.GetParentPath()
                    .Append(parentElement)
                    .Select(s => s.Name?.Replace(".", "_").ToPascalCase() ?? string.Empty))
            : "Default";
        Folder = parentElement?.ParentElement?.AsFolderModel();
        InternalElement = parentElement;
        Endpoints = elements
            .Select(element => new MediatREndpointModel(
                containerModel: this,
                endpoint: element,
                securedByDefault: securedByDefault,
                securityModels: SecurityModelHelpers.GetSecurityModels(element).ToArray()))
            .ToArray();
        ApplicableVersions = [];
    }

    public string Id { get; }
    public string Name { get; }
    public FolderModel? Folder { get; }
    public IElement? InternalElement { get; }
    public IReadOnlyCollection<IEndpointModel> Endpoints { get; }
    public IReadOnlyCollection<IApiVersionModel> ApplicableVersions { get; }
}

public class MediatREndpointModel : IEndpointModel
{
    public MediatREndpointModel(
        MediatREndpointContainerModel containerModel,
        IElement endpoint,
        bool securedByDefault,
        IReadOnlyCollection<ISecurityModel> securityModels)
    {
        if (!HttpEndpointModelFactory.TryGetEndpoint(
                element: endpoint,
                defaultBasePath: null,
                securedByDefault: securedByDefault,
                httpEndpointModel: out var httpEndpoint))
        {
            throw new InvalidOperationException("Could not obtain endpoint model");
        }

        Id = endpoint.Id;
        Comment = endpoint.Comment;
        Name = endpoint.Name;
        TypeReference = endpoint.TypeReference;
        Verb = httpEndpoint.Verb;
        Route = httpEndpoint.SubRoute;
        MediaType = httpEndpoint.MediaType;
        InternalElement = endpoint;
        Container = containerModel;
        Parameters = httpEndpoint.Inputs.Select(GetInput).ToList();
        RequiresAuthorization = httpEndpoint.RequiresAuthorization;
        AllowAnonymous = httpEndpoint.AllowAnonymous;
        SecurityModels = securityModels;
        ApplicableVersions = GetApplicableVersions(endpoint);
    }

    public string Id { get; }
    public string Comment { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public ITypeReference? ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string? Route { get; }
    public HttpMediaType? MediaType { get; }
    public IElement InternalElement { get; }
    public IEndpointContainerModel Container { get; }
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

    private static IApiVersionModel[] GetApplicableVersions(IElement element)
    {
        if (element.IsCommandModel())
        {
            return element.AsCommandModel().GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new EndpointApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToArray() ?? [];
        }

        if (element.IsQueryModel())
        {
            return element.AsQueryModel().GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new EndpointApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToArray() ?? [];
        }

        return [];
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