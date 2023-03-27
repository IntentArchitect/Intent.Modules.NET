using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Types.Api;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;

public class ServiceControllerModel : IControllerModel
{
    private readonly ServiceModel _model;

    public ServiceControllerModel(ServiceModel model)
    {
        if (model.HasSecured() && model.HasUnsecured())
        {
            throw new Exception($"Controller {model.Name} cannot require authorization and allow-anonymous at the same time");
        }
        _model = model;
        RequiresAuthorization = model.HasSecured();
        AllowAnonymous = model.HasUnsecured();
        Route = string.IsNullOrWhiteSpace(model.GetHttpServiceSettings().Route())
            ? "api/[controller]"
            : model.GetHttpServiceSettings().Route();
        Operations = model.Operations
            .Where(x => x.HasHttpSettings())
            .Select(x => new ControllerOperationModel(
                name: x.Name,
                element: x.InternalElement,
                verb: Enum.TryParse(x.GetHttpSettings().Verb().Value, ignoreCase: true, out HttpVerb verbEnum) ? verbEnum : HttpVerb.Post,
                route: x.GetHttpSettings().Route(),
                mediaType: Enum.TryParse(x.GetHttpSettings().ReturnTypeMediatype().Value, ignoreCase: true, out MediaTypeOptions mediaType) ? mediaType : MediaTypeOptions.Default,
                requiresAuthorization: x.HasSecured(),
                allowAnonymous: x.HasUnsecured(),
                authorizationModel: new AuthorizationModel()
                {
                    RolesExpression = x.HasSecured()
                        ? @$"""{string.Join(",", x.GetSecured().Roles()
                            .Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => s.Trim()))}"""
                        : null
                },
                parameters: x.Parameters
                    .Select(p => new ControllerParameterModel(id: p.Id, name: p.Name, typeReference: p.TypeReference, source: Enum.Parse<SourceOptionsEnum>(p.GetParameterSettings().Source().Value), headerName: p.GetParameterSettings().HeaderName(), mappedPayloadProperty: p.InternalElement.MappedElement?.Element))
                    .ToList<IControllerParameterModel>()
            ))
            .ToList<IControllerOperationModel>();
    }

    public string Id => _model.Id;
    public FolderModel Folder => _model.Folder;
    public string Name => _model.Name;
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel AuthorizationModel { get; }
    public string Comment => _model.Comment;
    public string Route { get; }
    public IList<IControllerOperationModel> Operations { get; }
}


public class ControllerOperationModel : IControllerOperationModel
{
    //public ControllerOperationModel(OperationModel model)
    //{
    //    Id = model.Id;
    //    Name = model.Name;
    //    Comment = model.Comment;
    //    TypeReference = model.TypeReference;
    //    Verb = Enum.TryParse(model.GetHttpSettings().Verb().Value, ignoreCase: true, out HttpVerb verbEnum) ? verbEnum : HttpVerb.Post;
    //    Route = model.GetHttpSettings().Route();
    //    MediaType = Enum.TryParse(model.GetHttpSettings().ReturnTypeMediatype().Value, ignoreCase: true, out MediaTypeOptions mediaType) ? mediaType : MediaTypeOptions.Default;
    //    RequiresAuthorization = model.HasSecured();
    //    AllowAnonymous = model.HasUnsecured();
    //    AuthorizationModel = new AuthorizationModel()
    //    {
    //        RolesExpression = model.HasSecured() ? @$"""{string.Join(",", model.GetSecured().Roles()
    //            .Split(',', StringSplitOptions.RemoveEmptyEntries)
    //            .Select(s => s.Trim()))}""" : null
    //    };
    //    InternalElement = model.InternalElement;
    //    Parameters = model.Parameters
    //        .Where(x => true)
    //        .Select(x => new ControllerParameterModel(x.Id, x.Name, x.TypeReference, Enum.Parse<SourceOptionsEnum>(x.GetParameterSettings().Source().Value), x.GetParameterSettings().HeaderName(), x.InternalElement.MappedElement?.Element))
    //        .ToList<IControllerParameterModel>();
    //}

    public ControllerOperationModel(
        string name,
        IElement element,
        HttpVerb verb,
        string route,
        MediaTypeOptions mediaType,
        bool requiresAuthorization,
        bool allowAnonymous,
        IAuthorizationModel authorizationModel,
        IList<IControllerParameterModel> parameters)
    {
        Id = element.Id;
        Name = name;
        TypeReference = element.TypeReference;
        Comment = element.Comment;
        InternalElement = element;
        Verb = verb;
        Route = route;
        MediaType = mediaType;
        RequiresAuthorization = requiresAuthorization;
        AllowAnonymous = allowAnonymous;
        AuthorizationModel = authorizationModel;
        Parameters = parameters;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public string Comment { get; }
    public IElement InternalElement { get; }
    public ITypeReference ReturnType => TypeReference.Element != null ? TypeReference : null;
    public HttpVerb Verb { get; }
    public string Route { get; }
    public MediaTypeOptions MediaType { get; }
    public bool RequiresAuthorization { get; }
    public bool AllowAnonymous { get; }
    public IAuthorizationModel AuthorizationModel { get; }
    public IList<IControllerParameterModel> Parameters { get; }
}

public class ControllerParameterModel : IControllerParameterModel
{
    public ControllerParameterModel(
        string id,
        string name,
        ITypeReference typeReference,
        SourceOptionsEnum source,
        string headerName,
        ICanBeReferencedType mappedPayloadProperty)
    {
        Id = id;
        Name = name;
        TypeReference = typeReference;
        Source = source;
        HeaderName = headerName;
        MappedPayloadProperty = mappedPayloadProperty;
    }

    public string Id { get; }
    public string Name { get; }
    public ITypeReference TypeReference { get; }
    public SourceOptionsEnum Source { get; }
    public string HeaderName { get; }
    public ICanBeReferencedType MappedPayloadProperty { get; }
}