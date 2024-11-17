#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;

public class ServiceControllerModel : IControllerModel
{
    private readonly ServiceModel _model;

    public ServiceControllerModel(ServiceModel model, bool securedByDefault)
    {
        if (!HttpEndpointModelFactory.TryGetCollection(
                element: model.InternalElement,
                defaultBasePath: null,
                securedByDefault: securedByDefault,
                out var endpointCollectionModel))
        {
            throw new ElementException(model.InternalElement, $"An error occured while trying to process the controller {model.Name}");
        }

        if (model.HasSecured() && model.HasUnsecured())
        {
            throw new ElementException(model.InternalElement, $"Controller {model.Name} cannot require authorization and allow-anonymous at the same time");
        }

        _model = model;
        RequiresAuthorization = model.HasSecured();
        AllowAnonymous = model.HasUnsecured();
        SecurityModels = endpointCollectionModel.SecurityModels;
        Route = GetControllerRoute(model.GetHttpServiceSettings()?.Route());
        Operations = endpointCollectionModel.Endpoints
            .Select(GetOperation)
            .ToList();
        ApplicableVersions = model.GetApiVersionSettings()
            ?.ApplicableVersions()
            .Select(s => new ControllerApiVersionModel(s))
            .Cast<IApiVersionModel>()
            .ToList() ?? [];
        InternalElement = model.InternalElement;
    }

    private string? GetControllerRoute(string? route)
    {
        if (string.IsNullOrWhiteSpace(route))
        {
            return null;
        }

        var serviceName = _model.Name.RemoveSuffix("Controller", "Service");
        var segments = route
            .Split('/')
            .Select(segment => segment.Equals(serviceName, StringComparison.OrdinalIgnoreCase) ? "[controller]" : segment);

        return string.Join('/', segments);
    }

    private IControllerOperationModel GetOperation(IHttpEndpointModel httpEndpoint)
    {
        var model = httpEndpoint.InternalElement.AsOperationModel();

        return new ControllerOperationModel(
            httpEndpoint: httpEndpoint,
            applicableVersions: model.GetApiVersionSettings()?
                .ApplicableVersions()
                .Select(s => new ControllerApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToList() ?? [],
            controller: this);
    }

    public string Id => _model.Id;
    public FolderModel Folder => _model.Folder;
    public string Name => _model.Name;
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public string Comment => _model.Comment;
    public string? Route { get; }
    public IList<IControllerOperationModel> Operations { get; }
    public IList<IApiVersionModel> ApplicableVersions { get; }
    public IElement InternalElement { get; }
}


public class ControllerOperationModel : IControllerOperationModel
{
    public ControllerOperationModel(
        IHttpEndpointModel httpEndpoint,
        IList<IApiVersionModel> applicableVersions,
        IControllerModel controller)
    {
        Name = httpEndpoint.Name;
        InternalElement = httpEndpoint.InternalElement;
        Verb = httpEndpoint.Verb;
        Route = httpEndpoint.SubRoute;
        MediaType = httpEndpoint.MediaType;
        RequiresAuthorization = httpEndpoint.RequiresAuthorization;
        AllowAnonymous = httpEndpoint.AllowAnonymous;
        SecurityModels = httpEndpoint.SecurityModels;
        Parameters = httpEndpoint.Inputs.Select(GetInput).ToList();
        ApplicableVersions = applicableVersions;
        Controller = controller;
    }

    private static IControllerParameterModel GetInput(IHttpEndpointInputModel model)
    {
        return new ControllerParameterModel(
            id: model.Id,
            name: model.Name,
            typeReference: model.TypeReference,
            source: model.Source,
            headerName: model.HeaderName,
            queryStringName: model.QueryStringName,
            mappedPayloadProperty: model.MappedPayloadProperty,
            value: model.Value);
    }

    public string Id => InternalElement.Id;
    public string Name { get; }
    public ITypeReference TypeReference => InternalElement.TypeReference;
    public string Comment => InternalElement.Comment;
    public IElement InternalElement { get; }
    public ITypeReference? ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string? Route { get; }
    public HttpMediaType? MediaType { get; }
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public IList<IControllerParameterModel> Parameters { get; }
    public IList<IApiVersionModel> ApplicableVersions { get; }
    public IControllerModel Controller { get; }
}

public class ControllerParameterModel : IControllerParameterModel
{
    public ControllerParameterModel(
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

public class ControllerApiVersionModel : IApiVersionModel
{
    public ControllerApiVersionModel(string definitionName, string version, bool isDeprecated)
    {
        DefinitionName = definitionName;
        Version = version;
        IsDeprecated = isDeprecated;
    }

    public ControllerApiVersionModel(ICanBeReferencedType element)
    {
        var versionModel = element.AsVersionModel();
        if (versionModel == null)
        {
            throw new InvalidOperationException($"Element {element.Id} [{element.Name}] is not a VersionModel.");
        }

        DefinitionName = versionModel.ApiVersion?.Name;
        Version = versionModel.Name;
        IsDeprecated = versionModel.GetVersionSettings()?.IsDeprecated() == true;
    }

    public string? DefinitionName { get; }
    public string Version { get; }
    public bool IsDeprecated { get; }

    protected bool Equals(ControllerApiVersionModel other)
    {
        return DefinitionName == other.DefinitionName &&
               Version == other.Version &&
               IsDeprecated == other.IsDeprecated;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((ControllerApiVersionModel)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(DefinitionName, Version, IsDeprecated);
    }
}