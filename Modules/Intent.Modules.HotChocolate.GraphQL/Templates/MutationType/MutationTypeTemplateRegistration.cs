using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.HotChocolate.GraphQL.Models;
using Intent.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using GraphQLMutationTypeModel = Intent.Modules.HotChocolate.GraphQL.Models.GraphQLMutationTypeModel;
using GraphQLQueryTypeModel = Intent.Modules.HotChocolate.GraphQL.Models.GraphQLQueryTypeModel;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.MutationType
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class MutationTypeTemplateRegistration : FilePerModelTemplateRegistration<IGraphQLMutationTypeModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MutationTypeTemplateRegistration(IMetadataManager metadataManager)
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
            return _metadataManager.Services(application).GetGraphQLMutationTypeModels()
                .Select(x => new GraphQLMutationTypeModel(x))
                .ToList();
        }
    }
}