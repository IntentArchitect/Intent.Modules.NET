using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.Dtos.Templates;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.MediatR.Templates.QueryModels;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class QueryHandlerTemplate : CSharpTemplateBase<QueryModel, QueryHandlerDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.QueryHandler";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryHandlerTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource(DtoModelTemplate.TemplateId, "List<{0}>");
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Handler",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        private string GetQueryModelName()
        {
            return GetTypeName(QueryModelsTemplate.TemplateId, Model);
        }

        private string GetFields()
        {
            return $@"
        {string.Join(@"
        ", GetDecorators().SelectMany(x => x.GetRequiredServices()).Distinct().Select(x => $"private {x.Type} _{x.Name.ToCamelCase()};"))}";
        }

        private string GetCtorParams()
        {
            return string.Join(@", ", GetDecorators().SelectMany(x => x.GetRequiredServices()).Distinct().Select(x => $"{x.Type} {x.Name.ToCamelCase()}"));
        }

        private string GetCtorInitializations()
        {
            return $@"
            {string.Join(@"
            ", GetDecorators().SelectMany(x => x.GetRequiredServices()).Distinct().Select(x => $"_{x.Name.ToCamelCase()} = {x.Name.ToCamelCase()};"))}";
        }

        private string GetImplementation()
        {
            var impl = GetDecoratorsOutput(x => x.GetImplementation());
            return !string.IsNullOrWhiteSpace(impl) ? impl : @"
            throw new NotImplementedException(""Your implementation here..."");";
        }
    }
}