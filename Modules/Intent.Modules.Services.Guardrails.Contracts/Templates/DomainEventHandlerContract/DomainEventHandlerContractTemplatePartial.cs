using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.DomainEventHandlerContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventHandlerContractTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.DomainEventHandlerContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventHandlerContractTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            // Types are only resolved here when this handler owns a multi-type file shared with other contracts.
            this.AddContractTypeSources();

            var fileConfig = Model.GetDomainEventHandlerFileConfig();
            CSharpFile = new CSharpFile(this.GetContractNamespace(Model, fileConfig?.Namespace(), fileConfig?.NoNamespace() == true), Model.GetElementFolderPath(), this)
                .IntentManagedContractMerge();

            ContractTypeBuilders.AddDomainEventHandler(CSharpFile, this, Model);
            CSharpFile.WithContractFileName(Model, fileConfig?.Filename());
            this.RegisterContractFileGroup();
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
    }
}