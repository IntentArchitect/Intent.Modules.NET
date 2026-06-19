using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.Endpoint.Models;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Dispatch.Wolverine.Templates.Endpoint;

public class WolverineEndpointContainerModel : IEndpointContainerModel
{
    public WolverineEndpointContainerModel(
        IElement? parentElement,
        IEnumerable<IElement> elements,
        ISoftwareFactoryExecutionContext context)
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
            .Select(element => new WolverineEndpointModel(
                containerModel: this,
                endpoint: element,
                context: context,
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

public class WolverineEndpointModel : IEndpointModel
{
    public WolverineEndpointModel(
        WolverineEndpointContainerModel containerModel,
        IElement endpoint,
        ISoftwareFactoryExecutionContext context,
        IReadOnlyCollection<ISecurityModel> securityModels)
    {
        if (!context.TryGetHttpEndpoint(
                element: endpoint,
                defaultBasePath: null,
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
        return new WolverineEndpointParameterModel(
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

public class WolverineEndpointParameterModel : IEndpointParameterModel
{
    public WolverineEndpointParameterModel(
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
