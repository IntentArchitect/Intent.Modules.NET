#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.ImplicitControllers;

public class CqrsControllerModel : IControllerModel
{
    public CqrsControllerModel(IElement? folderElement, IPackage package, ISoftwareFactoryExecutionContext context)
    {
        string name;
        IHttpEndpointCollectionModel endpointCollectionModel;

        if (folderElement != null)
        {
            name = string.Concat(folderElement
                .GetParentPath()
                .Append(folderElement)
                .Select(x => x.Name?.Replace(".", "_").ToPascalCase()));
            if (!context.TryGetHttpEndpointCollection(
                    element: folderElement,
                    defaultBasePath: null,
                    out endpointCollectionModel!))
            {
                throw new ElementException(folderElement, $"An error occured while trying to process the controller for {folderElement.Name}");
            }
        }
        else
        {
            name = "Default";
            if (!context.TryGetHttpEndpointCollection(
                    package: package,
                    defaultBasePath: null,
                    out endpointCollectionModel!))
            {
                throw new Exception($"An error occured while trying to process the controller for package {package.Name}");
            }
        }

        Id = folderElement?.Id ?? Guid.Empty.ToString();
        Name = name;
        Folder = folderElement?.ParentElement?.AsFolderModel();
        Operations = endpointCollectionModel.Endpoints.Select(MapToOperation).ToList();
        ApplicableVersions = new List<IApiVersionModel>();
        AllowAnonymous = endpointCollectionModel.AllowAnonymous;
        RequiresAuthorization = endpointCollectionModel.RequiresAuthorization;
        SecurityModels = endpointCollectionModel.SecurityModels;
    }
    
    [Obsolete]
    public CqrsControllerModel(IElement? folderElement, IPackage package, bool securedByDefault)
    {
        string name;
        IHttpEndpointCollectionModel endpointCollectionModel;

        if (folderElement != null)
        {
            name = string.Concat(folderElement
                .GetParentPath()
                .Append(folderElement)
                .Select(x => x.Name?.Replace(".", "_").ToPascalCase()));
            if (!HttpEndpointModelFactory.TryGetCollection(
                    element: folderElement,
                    defaultBasePath: null,
                    securedByDefault: securedByDefault,
                    out endpointCollectionModel!))
            {
                throw new ElementException(folderElement, $"An error occured while trying to process the controller for {folderElement.Name}");
            }
        }
        else
        {
            name = "Default";
            if (!HttpEndpointModelFactory.TryGetCollection(
                    package: package,
                    defaultBasePath: null,
                    securedByDefault: securedByDefault,
                    out endpointCollectionModel!))
            {
                throw new Exception($"An error occured while trying to process the controller for package {package.Name}");
            }
        }

        Id = folderElement?.Id ?? Guid.Empty.ToString();
        Name = name;
        Folder = folderElement?.ParentElement?.AsFolderModel();
        Operations = endpointCollectionModel.Endpoints.Select(MapToOperation).ToList();
        ApplicableVersions = new List<IApiVersionModel>();
        AllowAnonymous = endpointCollectionModel.AllowAnonymous;
        RequiresAuthorization = endpointCollectionModel.RequiresAuthorization;
        SecurityModels = endpointCollectionModel.SecurityModels;
    }

    private IControllerOperationModel MapToOperation(IHttpEndpointModel httpEndpoint)
    {
        return new ControllerOperationModel(
            httpEndpoint: httpEndpoint,
            applicableVersions: GetApplicableVersions(httpEndpoint.InternalElement),
            controller: this);
    }

    private static List<IApiVersionModel> GetApplicableVersions(IElement element)
    {
        if (element.IsCommandModel())
        {
            return element.AsCommandModel().GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new ControllerApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToList() ?? [];
        }

        if (element.IsQueryModel())
        {
            return element.AsQueryModel().GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new ControllerApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToList() ?? [];
        }

        return new List<IApiVersionModel>();
    }

    public string Id { get; }
    public FolderModel? Folder { get; }
    public string Name { get; }
    public string? Comment => null;
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IReadOnlyCollection<ISecurityModel> SecurityModels { get; }
    public string? Route => null;
    public IList<IControllerOperationModel> Operations { get; }
    public IList<IApiVersionModel> ApplicableVersions { get; }
}