using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.QueryContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class QueryContractTemplate : CSharpTemplateBase<QueryModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.QueryContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public QueryContractTemplate(IOutputTarget outputTarget, QueryModel model) : base(TemplateId, outputTarget, model)
        {
            this.AddContractTypeSources();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .IntentManagedMerge()
                .AddContractType(Model.GetContractTypeKind(), Model.Name, @class =>
                {
                    @class.RepresentsModel(Model);
                    @class.AddContractProperties(this, Model.Properties);
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