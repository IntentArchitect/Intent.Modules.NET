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

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.DomainEventContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainEventContractTemplate : CSharpTemplateBase<IElement>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Domain.Guardrails.Contracts.DomainEventContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainEventContractTemplate(IOutputTarget outputTarget, IElement model = null) : base(TemplateId, outputTarget, model)
        {
            this.ConfigureContractTypeResolution();

            // A raw IElement model is not IHasFolder, so GetNamespace()/GetFolderPath() would ignore its designer folders.
            CSharpFile = new CSharpFile(ContractFileGroups.GetContractNamespace(Model, Model.GetFileConfigNamespace(), Model.GetFileConfigNoNamespace(), this.GetElementNamespace(Model)), Model.GetElementFolderPath(), this)
                .IntentManagedContractMerge();

            ContractTypeBuilders.AddDomainEvent(CSharpFile, this, Model);
            CSharpFile.WithContractFileName(Model, Model.GetFileConfigFilename());
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