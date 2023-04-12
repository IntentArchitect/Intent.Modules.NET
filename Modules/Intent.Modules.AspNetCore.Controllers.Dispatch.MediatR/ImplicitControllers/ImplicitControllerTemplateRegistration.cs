using System;
using System.Collections.Generic;
using System.Linq;
using Intent.AspNetCore.Controllers.Dispatch.MediatR.Api;
using Intent.Engine;
using Intent.Metadata;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Dispatch.MediatR.ImplicitControllers
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class ImplicitControllerTemplateRegistration : FilePerModelTemplateRegistration<IControllerModel>
    {
        private readonly IMetadataManager _metadataManager;

        public ImplicitControllerTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => ControllerTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IControllerModel model)
        {
            return new ControllerTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IControllerModel> GetModels(IApplication application)
        {
            var commands = _metadataManager.Services(application).GetCommandModels()
                .Where(x => x.HasHttpSettings())
                .GroupBy(x => (Id: x.Folder?.Id ?? Guid.Empty.ToString(), Name: x.Folder?.Name ?? ""), x => x)
                .ToDictionary(x => x.Key, x => x.ToList());
            var queries = _metadataManager.Services(application).GetQueryModels()
                .Where(x => x.HasHttpSettings())
                .GroupBy(x => (Id: x.Folder?.Id ?? Guid.Empty.ToString(), Name: x.Folder?.Name ?? ""), x => x)
                .ToDictionary(x => x.Key, x => x.ToList());

            var results = commands.Keys.Union(queries.Keys)
                .Select(x => new MediatRControllerModel(
                    id: x.Id,
                    name: x.Name,
                    commands: commands.TryGetValue((x.Id, x.Name), out var c) ? c : new List<CommandModel>(),
                    queries: queries.TryGetValue((x.Id, x.Name), out var q) ? q : new List<QueryModel>()))
                .ToArray<IControllerModel>();

            return results;
        }
    }
}