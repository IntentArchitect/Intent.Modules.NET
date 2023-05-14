using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.HotChocolate.GraphQL.Dispatch.Services.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Dispatch.Services.ImplicitResolvers;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryType;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Dispatch.MediatR.ImplicitResolvers
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class QueryResolverTemplateRegistration : FilePerModelTemplateRegistration<IGraphQLQueryTypeModel>
    {
        private readonly IMetadataManager _metadataManager;

        public QueryResolverTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => QueryTypeTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IGraphQLQueryTypeModel model)
        {
            return new QueryTypeTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IGraphQLQueryTypeModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetServiceModels()
                .Where(x => x.Operations.Any(o => o.HasGraphQLEnabled()))
                .Select(x => new ServiceGraphQLQueryTypeModel(
                    id: x.Id,
                    name: $"{x.Name.RemoveSuffix("Service")}Queries",
                    resolvers: x.Operations.Where(o => o.HasGraphQLEnabled() &&
                                                       IsQueryOperation(o)).Select(o => new OperationGraphQLResolverModel(o)).ToList()))
                .ToList();
        }

        private static bool IsQueryOperation(OperationModel o)
        {
            return o.Name.StartsWith("Get") || o.Name.StartsWith("Find") || o.Name.StartsWith("Lookup");
        }
    }
}