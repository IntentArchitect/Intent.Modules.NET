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
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.CRUD.Tests.Templates.Nested.NestedCreateCommandHandlerTests
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class NestedCreateCommandHandlerTestsTemplateRegistration : FilePerModelTemplateRegistration<CommandModel>
    {
        private readonly IMetadataManager _metadataManager;

        public NestedCreateCommandHandlerTestsTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => NestedCreateCommandHandlerTestsTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, CommandModel model)
        {
            return new NestedCreateCommandHandlerTestsTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<CommandModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application)
                .GetCommandModels()
                .Where(p => p.Name.StartsWith("create", StringComparison.OrdinalIgnoreCase)
                            && p.Mapping?.Element.AsClassModel()?.IsAggregateRoot() == false)
                .ToList();
        }
    }
}