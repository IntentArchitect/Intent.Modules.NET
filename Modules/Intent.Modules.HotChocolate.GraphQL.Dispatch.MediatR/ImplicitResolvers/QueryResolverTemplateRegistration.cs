using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.HotChocolate.GraphQL.Dispatch.MediatR.Api;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Templates;
using Intent.Modules.HotChocolate.GraphQL.Templates.QueryResolver;
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

        public override string TemplateId => QueryResolverTemplate.TemplateId;

        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, IGraphQLQueryTypeModel model)
        {
            return new QueryResolverTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<IGraphQLQueryTypeModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetQueryModels()
                .Where(x => x.HasGraphQLEnabled())
                .GroupBy(x => x.InternalElement.ParentElement)
                .Select(x => new MediatRGraphQLQueryTypeModel($"{x.Key.Name.Singularize()}Queries", x.Select(q => new QueryGraphQLResolverModel(q)).ToList()))
                .ToList();
        }
    }
}