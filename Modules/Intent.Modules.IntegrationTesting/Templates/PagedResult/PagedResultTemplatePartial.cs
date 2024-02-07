using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Contracts.Clients.Shared.Templates.PagedResult;
using Intent.Modules.IntegrationTesting.Templates.HttpClient;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IntegrationTesting.Templates.PagedResult
{
    [IntentManaged(Mode.Ignore)]
    public partial class PagedResultTemplate : PagedResultTemplateBase, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.IntegrationTesting.PagedResult";

        public PagedResultTemplate(IOutputTarget outputTarget) : base(TemplateId, outputTarget)
        {
        }

        protected override IDesigner GetSourceDesigner(IMetadataManager metadataManager, string applicationId)
        {
            return null;
        }

        public override bool CanRunTemplate()
        {
            return ExecutionContext.FindTemplateInstances(HttpClientTemplate.TemplateId).Any();
        }
    }
}