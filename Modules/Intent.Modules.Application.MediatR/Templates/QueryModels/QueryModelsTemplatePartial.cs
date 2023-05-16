using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.Api;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.QueryModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class QueryModelsTemplate : CSharpTemplateBase<QueryModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.QueryModels";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public QueryModelsTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource("Application.Contract.Dto", "List<{0}>");
            AddTypeSource("Domain.Enum", "List<{0}>");
            FulfillsRole("Application.Contract.Query");
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{this.GetNamespace(additionalFolders: Model.GetConceptName())}",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}");
        }

        private string GetRequestInterface()
        {
            var interfaces = new List<string>();
            interfaces.Add($"IRequest<{GetTypeName(Model.TypeReference)}>");
            interfaces.Add(this.GetQueryInterfaceName());
            return string.Join(", ", interfaces);
        }

        private string GetComments(string indentation)
        {
            return TemplateHelper.GetXmlDocComments(Model.InternalElement?.Comment, indentation);
        }

        private string GetQueryAttributes()
        {
            if (Model.HasAuthorize())
            {
                var rolesPolicies = new List<string>();
                if (!string.IsNullOrWhiteSpace(Model.GetAuthorize().Roles()))
                {
                    rolesPolicies.Add($"Roles = \"{Model.GetAuthorize().Roles()}\"");
                }
                if (!string.IsNullOrWhiteSpace(Model.GetAuthorize().Policy()))
                {
                    rolesPolicies.Add($"Policy = \"{Model.GetAuthorize().Policy()}\"");
                }
                return $@"[{TryGetTypeName("Application.Identity.AuthorizeAttribute")?.RemoveSuffix("Attribute") ?? "Authorize"}{(rolesPolicies.Any() ? $"({string.Join(", ", rolesPolicies)})" : "")}]
    ";
            }

            return string.Empty;
        }
    }
}