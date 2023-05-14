using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Modelers.Services.GraphQL.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.Templates.QueryType
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class QueryTypeTemplateRegistration : FilePerModelTemplateRegistration<IGraphQLQueryTypeModel>
    {
        private readonly IMetadataManager _metadataManager;

        public QueryTypeTemplateRegistration(IMetadataManager metadataManager)
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
            return _metadataManager.Services(application).GetGraphQLQueryTypeModels()
                .Select(x => new GraphQLQueryTypeModel(x))
                .ToList();
        }
    }
}