using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.FilePerModel", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.QueryContract
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class QueryContractTemplateRegistration : FilePerModelTemplateRegistration<QueryModel>
    {
        private readonly IMetadataManager _metadataManager;

        public QueryContractTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => QueryContractTemplate.TemplateId;

        [IntentManaged(Mode.Fully)]
        public override ITemplate CreateTemplateInstance(IOutputTarget outputTarget, QueryModel model)
        {
            return new QueryContractTemplate(outputTarget, model);
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override IEnumerable<QueryModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetQueryModels();
        }
    }
}