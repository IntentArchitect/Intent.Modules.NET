using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.EnumContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EnumContractTemplate : CSharpTemplateBase<EnumModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.EnumContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EnumContractTemplate(IOutputTarget outputTarget, EnumModel model) : base(TemplateId, outputTarget, model)
        {
            this.AddContractTypeSources();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .IntentManagedMerge()
                .AddEnum(Model.Name, @enum =>
                {
                    // No RepresentsModel here: Intent.Modules.Common.CSharp (3.10.5-3.10.11) throws for CSharpEnum and CSharpEnumLiteral.
                    foreach (var literal in Model.Literals)
                    {
                        @enum.AddLiteral(literal.Name, string.IsNullOrWhiteSpace(literal.Value) ? null : literal.Value);
                    }
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