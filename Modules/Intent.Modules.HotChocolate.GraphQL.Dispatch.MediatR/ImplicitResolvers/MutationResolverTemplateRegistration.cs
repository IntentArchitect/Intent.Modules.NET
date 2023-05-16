using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.HotChocolate.GraphQL.Dispatch.MediatR.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MutationResolverTemplateRegistration : FilePerModelTemplateRegistration<IGraphQLMutationTypeModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MutationResolverTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => MutationTypeTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IGraphQLMutationTypeModel model)
        {
            return new MutationTypeTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IGraphQLMutationTypeModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetCommandModels()
                .Where(x => x.HasGraphQLEnabled())
                .GroupBy(x => x.InternalElement.ParentElement)
                .Select(x => new MediatRGraphQLMutationTypeModel(x.Key.Id, $"{x.Key.Name.Singularize()}Mutations", x.Select(q => new CommandGraphQLResolverModel(q)).ToList()))
                .ToList();
        }
    }
}