using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.MediatR.Api;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.Templates.CommandModels
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class CommandModelsTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.CommandModels";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandModelsTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile($"{this.GetNamespace(additionalFolders: Model.GetConceptName())}", $"{this.GetFolderPath(additionalFolders: Model.GetConceptName())}")
                .AddClass($"{Model.Name}", @class =>
                {
                    @class.TryAddXmlDocComments(Model.InternalElement);
                    AddAuthorization(@class);
                    @class.ImplementsInterface(Model.TypeReference.Element != null ? $"IRequest<{GetTypeName(Model.TypeReference)}>" : "IRequest");
                    @class.ImplementsInterface(this.GetCommandInterfaceName());

                    @class.AddConstructor(ctor =>
                    {
                        foreach (var property in Model.Properties)
                        {
                            ctor.AddParameter(GetTypeName(property), property.Name.ToParameterName(), p => 
                            {
                                p.IntroduceProperty();
                            });
                        }
                    });
                });
            AddNugetDependency(NuGetPackages.MediatR);
            AddTypeSource("Application.Contract.Dto", "List<{0}>");
            AddTypeSource("Domain.Enum", "List<{0}>");
            FulfillsRole("Application.Contract.Command");
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Fully)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return CSharpFile.GetConfig();
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }

        private void AddAuthorization(CSharpClass @class)
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
                @class.AddAttribute(TryGetTypeName("Application.Identity.AuthorizeAttribute")?.RemoveSuffix("Attribute") ?? "Authorize", att =>
                {
                    foreach (var arg in rolesPolicies)
                    {
                        att.AddArgument(arg);
                    }
                });
            }
        }
    }
}