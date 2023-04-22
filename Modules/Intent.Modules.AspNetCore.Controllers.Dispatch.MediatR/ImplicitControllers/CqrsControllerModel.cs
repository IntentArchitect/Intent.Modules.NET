using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Metadata.WebApi.Models;

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.ImplicitControllers;

public class CqrsControllerModel : IControllerModel
{
    public CqrsControllerModel(
        IElement parentElement,
        IEnumerable<IElement> elements)
    {
        Id = parentElement?.Id ?? Guid.Empty.ToString();
        Name = parentElement?.Name ?? "Default";
        Folder = parentElement?.ParentElement?.AsFolderModel();
        Operations = elements
            .Select(MapToOperation)
            .ToList();
    }

    private static IControllerOperationModel MapToOperation(IElement element)
    {
        var httpEndpointModel = HttpEndpointModelFactory.GetEndpoint(element)!;

        return new ControllerOperationModel(
            name: httpEndpointModel.Name,
            element: element,
            verb: httpEndpointModel.Verb,
            route: httpEndpointModel.SubRoute,
            mediaType: httpEndpointModel.MediaType,
            requiresAuthorization: httpEndpointModel.RequiresAuthorization,
            allowAnonymous: httpEndpointModel.AllowAnonymous,
            authorizationModel: null,
            parameters: httpEndpointModel.Inputs
                .Select(x => new ControllerParameterModel(
                    id: x.Id,
                    name: x.Name.ToCamelCase(),
                    typeReference: x.TypeReference,
                    source: x.Source,
                    headerName: x.HeaderName,
                    mappedPayloadProperty: x.MappedPayloadProperty))
                .ToList<IControllerParameterModel>());
    }

    public string Id { get; }
    public FolderModel Folder { get; }
    public string Name { get; }
    public string Comment => null;
    public bool RequiresAuthorization => false;
    public bool AllowAnonymous => false;
    public string Route => null;
    public IList<IControllerOperationModel> Operations { get; }
    public IAuthorizationModel AuthorizationModel => null;
}