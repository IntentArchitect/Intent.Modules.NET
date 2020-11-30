using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.MediatR.Templates.DtoModel;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class QueryModelsTemplate : CSharpTemplateBase<QueryModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Application.MediatR.QueryModels";

        public QueryModelsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource(DtoModelTemplate.TemplateId);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace()}.{Model.GetConceptName()}",
                relativeLocation: $"{this.GetFolderPath()}/{Model.GetConceptName()}");
        }

        private string GetRequestInterface()
        {
            return $"IRequest<{GetTypeName(Model.TypeReference)}>";
        }
    }
}