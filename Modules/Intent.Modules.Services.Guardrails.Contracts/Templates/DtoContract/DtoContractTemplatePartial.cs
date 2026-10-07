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

namespace Intent.Modules.Services.Guardrails.Contracts.Templates.DtoContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DtoContractTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Services.Guardrails.Contracts.DtoContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DtoContractTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            this.AddContractTypeSources();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath(), this)
                .IntentManagedMerge()
                .AddContractType(Model.GetContractTypeKind(), Model.Name, @class =>
                {
                    @class.RepresentsModel(Model);

                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (Model.ParentDtoTypeReference != null)
                    {
                        @class.WithBaseType(GetTypeName(Model.ParentDtoTypeReference));
                    }

                    @class.AddContractProperties(this, Model.Fields);
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