using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Settings;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using OperationModelExtensions = Intent.Modelers.Domain.Api.OperationModelExtensions;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedUpdateCommandHandlerTests
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class NestedUpdateCommandHandlerTestsTemplateRegistration : FilePerModelTemplateRegistration<CommandModel>
    {
        private readonly IMetadataManager _metadataManager;

        public NestedUpdateCommandHandlerTestsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => NestedUpdateCommandHandlerTestsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, CommandModel model)
        {
            return new NestedUpdateCommandHandlerTestsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<CommandModel> GetModels(IApplication application)
        {
            if (application.Settings.GetDomainSettings().EnsurePrivatePropertySetters())
            {
                return ArraySegment<CommandModel>.Empty;
            }

            return _metadataManager.Services(application)
                .GetCommandModels()
                // .Where(command => command.Name.StartsWith("update", StringComparison.OrdinalIgnoreCase)
                //             && command.GetClassModel()?.IsAggregateRoot() == false
                //             && command.HasIdentityKeys(application))
                .ToList();
        }
    }
}