using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates.Adapters;

internal class ImplicitServiceProxyModel : IServiceProxyModel
{
    public ImplicitServiceProxyModel(IReadOnlyList<IElement> elements)
    {
        var first = elements[0];
        Id = first.ParentId;
        InternalElement = first.ParentElement;
        Name = first.ParentElement != null
            ? first.ParentElement.Name.EnsureSuffixedWith("Service")
            : $"{first.Package.Name.RemoveSuffix(".Services").ToPascalCase().RemoveSuffix("Service")}DefaultService";
        Endpoints = elements
            .Select(HttpEndpointModelFactory.GetEndpoint)
            .OrderBy(x => x!.Name)
            .ThenBy(x => x!.Id)
            .ToArray();
        Folder = first.ParentElement?.SpecializationTypeId == FolderModel.SpecializationTypeId
            ? new FolderModel(first.ParentElement)
            : null;
        CreateParameterPerInput = elements.All(x => !x.IsCommandModel() && !x.IsQueryModel());
    }

    public string Id { get; }
    public FolderModel? Folder { get; }
    public IElement InternalElement { get; }
    public string Name { get; }
    public IReadOnlyList<IHttpEndpointModel> Endpoints { get; }
    public IMetadataModel UnderlyingModel => InternalElement;
    public bool CreateParameterPerInput { get; }
}