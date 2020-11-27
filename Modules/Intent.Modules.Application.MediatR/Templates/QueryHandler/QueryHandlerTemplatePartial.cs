using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class QueryHandlerTemplate : CSharpTemplateBase<Intent.Modelers.Services.CQRS.Api.QueryModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Modules.Application.MediatR.QueryHandler";

        public QueryHandlerTemplate(IOutputTarget outputTarget, Intent.Modelers.Services.CQRS.Api.QueryModel model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetQueryModelName()
        {
            return GetTypeName(QueryModelsTemplate.TemplateId, Model);
        }
    }
}