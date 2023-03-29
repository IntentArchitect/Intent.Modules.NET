using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.AspNetCore.Controllers.Dispatch.MediatR.Api;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeResolution;
using Intent.Modules.Common.Types.Api;
using static Intent.Modules.AspNetCore.Controllers.Templates.Controller.ControllerTemplate;

namespace Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;

public class MediatRControllerModel : IControllerModel
{
    public MediatRControllerModel(string id, string name, IList<CommandModel> commands, IList<QueryModel> queries)
    {
        Id = id;
        Name = name;
        Operations = commands
            .Select(command => new ControllerOperationModel(
                name: command.Name.RemoveSuffix("Command"),
                element: command.InternalElement,
                verb: Enum.TryParse(command.GetHttpSettings().Verb().AsEnum().ToString(), ignoreCase: true, out HttpVerb verbEnum) ? verbEnum : HttpVerb.Post,
                route: command.GetHttpSettings().Route(),
                mediaType: Enum.TryParse(command.GetHttpSettings().ReturnTypeMediatype().AsEnum().ToString(), out MediaTypeOptions mediaType) ? mediaType : MediaTypeOptions.Default,
                requiresAuthorization: false,
                allowAnonymous: false,
                authorizationModel: new AuthorizationModel(),
                parameters: command.Properties.Where(x => x.HasParameterSettings() || command.GetHttpSettings().Route()?.Contains($"{{{x.Name.ToCamelCase()}}}") == true)
                    .Select(x => new ControllerParameterModel(
                        id: x.Id,
                        name: x.Name.ToCamelCase(),
                        typeReference: x.TypeReference,
                        source: Enum.TryParse(x.GetParameterSettings()?.Source().AsEnum().ToString(), true, out SourceOptionsEnum sourceOptions) ? sourceOptions : SourceOptionsEnum.Default,
                        headerName: x.GetParameterSettings()?.HeaderName(),
                        mappedPayloadProperty: x.InternalElement))
                    .Concat(new List<IControllerParameterModel>()
                    {
                        new ControllerParameterModel(id: command.Id,
                            name: "command",
                            typeReference: command.InternalElement.AsTypeReference(),
                            source: SourceOptionsEnum.FromBody,
                            headerName: null,
                            mappedPayloadProperty: null)
                    }.Where(x => command.Properties.Count > command.Properties.Count(p => p.HasParameterSettings() || command.GetHttpSettings().Route()?.Contains($"{{{p.Name.ToCamelCase()}}}") == true)))
                    .ToList()
            )).Concat(queries
                .Select(query => new ControllerOperationModel(
                    name: query.Name.RemoveSuffix("Query"),
                    element: query.InternalElement,
                    verb: Enum.TryParse(query.GetHttpSettings().Verb().AsEnum().ToString(), ignoreCase: true, out HttpVerb verbEnum) ? verbEnum : HttpVerb.Get,
                    route: query.GetHttpSettings().Route(),
                    mediaType: Enum.TryParse(query.GetHttpSettings().ReturnTypeMediatype().AsEnum().ToString(), out MediaTypeOptions mediaType) ? mediaType : MediaTypeOptions.Default,
                    requiresAuthorization: false,
                    allowAnonymous: false,
                    authorizationModel: new AuthorizationModel(),
                    parameters: query.Properties.Where(x => x.HasParameterSettings() || query.GetHttpSettings().Verb().IsGET())
                        .Select(x => new ControllerParameterModel(
                            id: x.Id,
                            name: x.Name.ToCamelCase(),
                            typeReference: x.TypeReference,
                            source: Enum.TryParse(x.GetParameterSettings()?.Source().AsEnum().ToString(), true, out SourceOptionsEnum sourceOptions) ? sourceOptions : SourceOptionsEnum.Default,
                            headerName: x.GetParameterSettings()?.HeaderName(),
                            mappedPayloadProperty: x.InternalElement))
                        .Concat(new List<IControllerParameterModel>()
                        {
                            new ControllerParameterModel(id: query.Id,
                                name: "query",
                                typeReference: query.InternalElement.AsTypeReference(),
                                source: SourceOptionsEnum.FromBody,
                                headerName: null,
                                mappedPayloadProperty: null)
                        }.Where(x => !query.GetHttpSettings().Verb().IsGET()))
                        .ToList()
                )))
            .ToList<IControllerOperationModel>();
    }

    public string Id { get; }
    public FolderModel Folder => null;
    public string Name { get; }
    public string Comment => null;
    public bool RequiresAuthorization { get; } = false;
    public bool AllowAnonymous { get; } = false;
    public string Route { get; } = null;
    public IList<IControllerOperationModel> Operations { get; }
    public IAuthorizationModel AuthorizationModel { get; } = null;
}