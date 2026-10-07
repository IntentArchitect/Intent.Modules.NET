using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Domain.ValueObjects.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.ValueObjectContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValueObjectContractTemplate : CSharpTemplateBase<ValueObjectModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Domain.Guardrails.Contracts.ValueObjectContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValueObjectContractTemplate(IOutputTarget outputTarget, ValueObjectModel model) : base(TemplateId, outputTarget, model)
        {
            this.ConfigureContractTypeResolution();
            var kind = Model.GetContractTypeKind();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .IntentManagedMerge()
                .AddContractType(Model.InternalElement, Model.Name, kind, @class =>
                {
                    @class.RepresentsModel(Model);
                    @class.AddContractProperties(this, Model.Attributes);
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