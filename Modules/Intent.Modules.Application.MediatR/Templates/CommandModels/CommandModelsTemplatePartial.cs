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

namespace Intent.Modules.Application.MediatR.Templates.CommandModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CommandModelsTemplate : CSharpTemplateBase<CommandModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.CommandModels";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandModelsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource("Application.Contract.Dto", "List<{0}>");
            AddTypeSource("Domain.Enum", "List<{0}>");
            FulfillsRole("Application.Contract.Command");
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
            interfaces.Add(Model.TypeReference.Element != null ? $"IRequest<{GetTypeName(Model.TypeReference)}>" : "IRequest");
            interfaces.Add(this.GetCommandInterfaceName());
            return string.Join(", ", interfaces);
        }

        private string GetCommandAttributes()
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