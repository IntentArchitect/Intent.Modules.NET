using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;
using OperationModel = Intent.Modelers.Services.Api.OperationModel;

namespace Intent.Modules.FastEndpoints.Templates.Endpoint.Models;

public class ServiceEndpointContainerModel : IEndpointContainerModel
{
    public ServiceEndpointContainerModel(ServiceModel serviceModel)
    {
        Id = serviceModel.Id;
        Name = serviceModel.Name;
        Folder = serviceModel.Folder;
        InternalElement = serviceModel.InternalElement;
        Endpoints = serviceModel.Operations
            .Select(IEndpointModel (operation) => new ServiceEndpointModel(this, serviceModel, operation, GetAuthorizationModel(operation.InternalElement)))
            .ToList();
        RequiresAuthorization = serviceModel.HasSecured();
        AllowAnonymous = serviceModel.HasUnsecured();
        Authorization = GetAuthorizationModel(serviceModel.InternalElement);
    }

    public string Id { get; }
    public string Name { get; }
    public FolderModel Folder { get; }
    public IElement InternalElement { get; }
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