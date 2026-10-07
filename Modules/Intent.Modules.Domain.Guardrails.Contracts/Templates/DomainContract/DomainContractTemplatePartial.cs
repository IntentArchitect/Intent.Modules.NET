using System;
using System.Collections.Generic;
using Intent.Domain.Guardrails.Contracts.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.DomainContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DomainContractTemplate : CSharpTemplateBase<DataContractModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Domain.Guardrails.Contracts.DomainContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DomainContractTemplate(IOutputTarget outputTarget, DataContractModel model) : base(TemplateId, outputTarget, model)
        {
            this.ConfigureContractTypeResolution();
            var fileConfig = Model.GetFileConfig();

            CSharpFile = new CSharpFile(ContractFileGroups.GetContractNamespace(Model.InternalElement, fileConfig?.Namespace(), fileConfig?.NoNamespace() == true, this.GetNamespace()), this.GetFolderPath(), this)
                .IntentManagedMerge();

            ContractTypeBuilders.AddDataContract(CSharpFile, this, Model);
            CSharpFile.WithContractFileName(Model.InternalElement, fileConfig?.Filename());
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