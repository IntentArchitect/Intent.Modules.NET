using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
using Intent.Modules.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using GraphQLMutationTypeModel = Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver.GraphQLMutationTypeModel;
using GraphQLQueryTypeModel = Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver.GraphQLQueryTypeModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.MutationResolver
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MutationResolverTemplateRegistration : FilePerModelTemplateRegistration<IGraphQLMutationTypeModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MutationResolverTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => MutationResolverTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IGraphQLMutationTypeModel model)
        {
            return new MutationResolverTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IGraphQLMutationTypeModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetGraphQLMutationTypeModels()
                .Select(x => new GraphQLMutationTypeModel(x))
                .ToList();
        }
    }
}