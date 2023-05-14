using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.HotChocolate.GraphQL.Dispatch.Services.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Dispatch.Services.ImplicitResolvers;
using Intent.Modules.HotChocolate.GraphQL.Templates.MutationType;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.Modules.Modelers.Services.GraphQL.Api;
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
            return _metadataManager.Services(application).GetServiceModels()
                .Where(x => x.Operations.Any(o => o.HasGraphQLEnabled()))
                .Select(x => new ServiceGraphQLMutationTypeModel(
                    id: x.Id,
                    name: $"{x.Name.RemoveSuffix("Service")}Mutations",
                    resolvers: x.Operations.Where(o => o.HasGraphQLEnabled() && IsMutationOperation(o)).Select(o => new OperationGraphQLResolverModel(o)).ToList()))
                .ToList();
        }

        private static bool IsMutationOperation(OperationModel o)
        {
            return !(o.Name.StartsWith("Get") || o.Name.StartsWith("Find") || o.Name.StartsWith("Lookup"));
        }
    }
}