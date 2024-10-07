using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common;
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
        Name = parentElement is not null
            ? string.Join(string.Empty,
                parentElement.GetParentPath().Concat(new[] { parentElement })
                    .Select(s => s.Name?.Replace(".", "_").ToPascalCase() ?? string.Empty))
            : "Default";
        Folder = parentElement?.ParentElement?.AsFolderModel();
        Operations = elements.Select(MapToOperation).ToList();
        ApplicableVersions = new List<IApiVersionModel>();
    }

    private IControllerOperationModel MapToOperation(IElement element)
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
            authorizationModel: GetAuthorizationModel(element),
            parameters: httpEndpointModel.Inputs
                .Select(x => new ControllerParameterModel(
                    id: x.Id,
                    name: x.Name.ToCamelCase(),
                    typeReference: x.TypeReference,
                    source: x.Source,
                    headerName: x.HeaderName,
                    queryStringName: x.QueryStringName,
                    mappedPayloadProperty: x.MappedPayloadProperty,
                    value: x.Value))
                .ToList<IControllerParameterModel>(),
            applicableVersions: GetApplicableVersions(element),
            controller: this);
    }

    private static IList<IApiVersionModel> GetApplicableVersions(IElement element)
    {
        if (element.IsCommandModel())
        {
            return element.AsCommandModel().GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new ControllerApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToList();
        }

        if (element.IsQueryModel())
        {
            return element.AsQueryModel().GetApiVersionSettings()
                ?.ApplicableVersions()
                .Select(s => new ControllerApiVersionModel(s))
                .Cast<IApiVersionModel>()
                .ToList();
        }

        return new List<IApiVersionModel>();
    }
    
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

    public string Id { get; }
    public FolderModel Folder { get; }
    public string Name { get; }
    public string Comment => null;
    public bool RequiresAuthorization => false;
    public bool AllowAnonymous => false;
    public string Route => null;
    public IList<IControllerOperationModel> Operations { get; }
    public IList<IApiVersionModel> ApplicableVersions { get; }
    public IAuthorizationModel AuthorizationModel => null;
}