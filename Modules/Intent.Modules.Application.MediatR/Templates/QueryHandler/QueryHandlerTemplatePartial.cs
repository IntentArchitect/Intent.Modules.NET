using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.DtoModel;
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
        public const string TemplateId = "Intent.Application.MediatR.QueryHandler";

        public QueryHandlerTemplate(IOutputTarget outputTarget, Intent.Modelers.Services.CQRS.Api.QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource(DtoModelTemplate.TemplateId);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{this.GetNamespace()}.{Model.GetConceptName()}",
                relativeLocation: $"{this.GetFolderPath()}/{Model.GetConceptName()}");
        }

        private string GetQueryModelName()
        {
            return GetTypeName(QueryModelsTemplate.TemplateId, Model);
        }
    }
}