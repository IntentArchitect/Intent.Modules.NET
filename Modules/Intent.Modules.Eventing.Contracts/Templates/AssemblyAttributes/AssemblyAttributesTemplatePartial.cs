using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence;
using Intent.Modelers.Eventing.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Api;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationCommand;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventDto;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventEnum;
using Intent.Modules.Eventing.Contracts.Templates.IntegrationEventMessage;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Formatting",
    "IDE0130:Namespace does not match folder structure.",
    Justification = "Message namespaces need to consistent between applications for deserialization to work",
    Scope = "namespaceanddescendants",
    Target = "Publish.CleanArchDapr.TestApplication.Eventing.Messages")]

namespace Intent.Modules.Eventing.Contracts.Templates.AssemblyAttributes
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class AssemblyAttributesTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Eventing.Contracts.AssemblyAttributes";
        private bool _afterTemplateRegistrationCalled;
        private readonly HashSet<string> _namespaces = [];

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AssemblyAttributesTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System.Diagnostics.CodeAnalysis")
                .OnBuild(file =>
                {
                    foreach (var @namespace in _namespaces.Order())
                    {
                        file.AddAssemblyAttribute("SuppressMessage", a =>
                        {
                            a.AddArgument("\"Formatting\"");
                            a.AddArgument("\"IDE0130:Namespace does not match folder structure.\"");
                            a.AddArgument($"Target = \"{@namespace}\"");
                            a.AddArgument("Scope = \"namespaceanddescendants\"");
                            a.AddArgument("Justification = \"Message namespaces need to be consistent between applications for deserialization to work\"");
                        });
                    }
                });
        }

        public override void AfterTemplateRegistration()
        {
            TryAddForTemplateType(IntegrationCommandTemplate.TemplateId);
            TryAddForTemplateType(IntegrationEventDtoTemplate.TemplateId);
            TryAddForTemplateType(IntegrationEventEnumTemplate.TemplateId);
            TryAddForTemplateType(IntegrationEventMessageTemplate.TemplateId);
            _afterTemplateRegistrationCalled = true;
        }

        public override bool CanRunTemplate()
        {
            return !_afterTemplateRegistrationCalled || _namespaces.Count > 0;
        }

        private void TryAddForTemplateType(string templateId)
        {
            var templates = OutputTarget.ExecutionContext.FindTemplateInstances<ITemplateWithModel>(templateId).ToArray();
            if (templates.Length == 0)
            {
                return;
            }

            var namespaces = templates
                .Select(template =>
                {
                    var classNamespace =
                        ((IElementWrapper)template.Model).InternalElement.Package.Name.ToCSharpNamespace() ??
                        throw new InvalidOperationException($"{templateId} for {template.Model} has no namespace");
                    var extendedNamespace = ((IHasFolder)template.Model).GetParentFolders().Where(x =>
                        {
                            if (string.IsNullOrWhiteSpace(x.Name))
                            {
                                return false;
                            }

                            return !x.HasFolderOptions() || x.GetFolderOptions().NamespaceProvider();
                        })
                        .Select(x => x.Name);

                    return classNamespace.Split('.').Concat(extendedNamespace).ToArray();
                })
                .ToArray();

            var minLength = namespaces.Min(x => x.Length);
            var commonParts = new List<string>();

            for (var i = 0; i < minLength; i++)
            {
                var namespacePart = namespaces[0][i];

                if (namespaces.Skip(1).Any(@namespace => @namespace[i] != namespacePart))
                {
                    continue;
                }

                commonParts.Add(namespacePart);
            }

            _namespaces.Add(string.Join('.', commonParts));
        }

        [IntentManaged(Mode.Fully)]
        public CSharpFile CSharpFile { get; }

        [IntentManaged(Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: "AssemblyAttributes",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: this.GetFolderPath());
        }

        [IntentManaged(Mode.Fully)]
        public override string TransformText()
        {
            return CSharpFile.ToString();
        }
    }
}