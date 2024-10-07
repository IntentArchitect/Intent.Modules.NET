using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.FastEndpoints.Templates.Endpoint;
using Intent.Modules.FastEndpoints.Templates.Endpoint.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.FastEndpoints.Dispatch.MediatR.Templates.Endpoint;

public class MediatREndpointContainerModel : IEndpointContainerModel
{
    public MediatREndpointContainerModel(
        IElement? parentElement,
        IEnumerable<IElement> elements)
    {
        Id = parentElement?.Id ?? Guid.Empty.ToString();
        Name = parentElement is not null
            ? string.Join(string.Empty,
                parentElement.GetParentPath().Concat(new[] { parentElement })
                    .Select(s => s.Name?.Replace(".", "_").ToPascalCase() ?? string.Empty))
            : "Default";
        Folder = parentElement?.ParentElement?.AsFolderModel();
        InternalElement = parentElement;
        Endpoints = elements.Select(IEndpointModel (operation) => new MediatREndpointModel(this, operation, GetAuthorizationModel(operation))).ToList();
    }

    public string Id { get; }
    public FolderModel? Folder { get; }
    public string Name { get; }
    public IElement? InternalElement { get; }
    public IList<IEndpointModel> Endpoints { get; }
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel? Authorization { get; }
    
    private static bool GetAuthorizationRolesAndPolicies(IElement element, out string roles, out string policy)
    {
        roles = null;
        policy = null;
        if (!element.HasStereotype("Authorize") && !element.HasStereotype("Secured"))
        {
            return false;
        }
        var auth = element.HasStereotype("Authorize") ? element.GetStereotype("Authorize") : element.GetStereotype("Secured");

        if (!string.IsNullOrEmpty(auth.GetProperty<string>("Roles", null)))
        {
            roles = auth.GetProperty<string>("Roles");
        }
        if (!string.IsNullOrEmpty(auth.GetProperty<string>("Policy", null)))
        {
            policy = auth.GetProperty<string>("Policy");
        }
        if (auth.GetProperty<IElement[]>("Security Roles", null) != null)
        {
            var elements = auth.GetProperty<IElement[]>("Security Roles");
            roles = string.Join(",", elements.Select(e => e.Name));
        }
        if (auth.GetProperty<IElement[]>("Security Policies", null) != null)
        {
            var elements = auth.GetProperty<IElement[]>("Security Policies");
            policy = string.Join(",", elements.Select(e => e.Name));
        }
        return roles != null || policy != null;
    }

    private static AuthorizationModel GetAuthorizationModel(IElement element)
    {
        if (!GetAuthorizationRolesAndPolicies(element, out var roles, out var policies))
        {
            return null;
        }
        return new AuthorizationModel
        {
            RolesExpression = !string.IsNullOrWhiteSpace(roles)
                ? @$"{string.Join("+", roles.Split('+', StringSplitOptions.RemoveEmptyEntries)
                    .Select(group => string.Join(",", group.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()))))}"
                : null,
            Policy = !string.IsNullOrWhiteSpace(policies)
                ? @$"{string.Join("+", policies.Split('+', StringSplitOptions.RemoveEmptyEntries)
                    .Select(group => string.Join(",", group.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim()))))}"
                : null
        };
    }
}

public class MediatREndpointModel : IEndpointModel
{
    public MediatREndpointModel(
        MediatREndpointContainerModel containerModel,
        IElement endpoint,
        IAuthorizationModel authorizationModel)
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
        RequiresAuthorization = httpEndpoint.RequiresAuthorization;
        AllowAnonymous = httpEndpoint.AllowAnonymous;
        Authorization = authorizationModel;
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
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel? Authorization { get; }
    public FolderModel? Folder => new FolderModel(Container.InternalElement, Container.InternalElement.SpecializationType);
    
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