using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.Security.Models;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class ServiceEndpointContainerModel : IEndpointContainerModel
{
    public ServiceEndpointContainerModel(ServiceModel serviceModel, bool securedByDefault)
    {
        Id = serviceModel.Id;
        Name = serviceModel.Name;
        Folder = serviceModel.Folder;
        InternalElement = serviceModel.InternalElement;
        Endpoints = serviceModel.Operations
            .Select(IEndpointModel (operation) => new ServiceEndpointModel(
                container: this,
                serviceModel: serviceModel,
                operationModel: operation,
                securedByDefault: securedByDefault,
                securityModels: SecurityModelHelpers.GetSecurityModels(operation.InternalElement).ToArray()))
            .ToArray();
        ApplicableVersions = serviceModel.GetApiVersionSettings()
            ?.ApplicableVersions()
            .Select(s => new EndpointApiVersionModel(s))
            .Cast<IApiVersionModel>()
            .ToArray() ?? [];
    }

    public string Id { get; }
    public string Name { get; }
    public FolderModel Folder { get; }
    public IElement InternalElement { get; }
    public IReadOnlyCollection<IEndpointModel> Endpoints { get; }
    public IReadOnlyCollection<IApiVersionModel> ApplicableVersions { get; }
}