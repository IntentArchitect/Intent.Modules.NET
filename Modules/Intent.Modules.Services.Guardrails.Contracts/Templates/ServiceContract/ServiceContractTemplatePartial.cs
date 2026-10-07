using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.ServiceContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ServiceContractTemplate : CSharpTemplateBase<ServiceModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.ServiceContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ServiceContractTemplate(IOutputTarget outputTarget, ServiceModel model) : base(TemplateId, outputTarget, model)
        {
            this.AddContractTypeSources();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .IntentManagedMerge()
                .AddContractType(Model.GetContractTypeKind(), Model.Name, @class =>
                {
                    @class.RepresentsModel(Model);
                    @class.AddStubbedOperations(this, Model.Operations);
                });
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