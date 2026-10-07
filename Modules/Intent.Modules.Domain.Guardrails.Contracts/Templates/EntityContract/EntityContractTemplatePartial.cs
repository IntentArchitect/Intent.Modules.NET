using System;
using System.Collections.Generic;
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

namespace Intent.Modules.Domain.Guardrails.Contracts.Templates.EntityContract
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class EntityContractTemplate : CSharpTemplateBase<ClassModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Domain.Guardrails.Contracts.EntityContract";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public EntityContractTemplate(IOutputTarget outputTarget, ClassModel model) : base(TemplateId, outputTarget, model)
        {
            this.ConfigureContractTypeResolution();
            var kind = Model.GetContractTypeKind();

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .IntentManagedMerge()
                .AddContractType(Model.InternalElement, Model.Name, kind, @class =>
                {
                    @class.RepresentsModel(Model);
                    foreach (var genericType in Model.GenericTypes)
                    {
                        @class.AddGenericParameter(genericType);
                    }

                    if (Model.IsAbstract)
                    {
                        @class.Abstract();
                    }

                    if (Model.ParentClassTypeReference != null)
                    {
                        @class.WithBaseType(GetTypeName(Model.ParentClassTypeReference));
                    }

                    @class.AddContractProperties(this, Model.Attributes);
                    @class.AddContractNavigationProperties(this, Model.AssociatedClasses);
                    @class.AddStubbedConstructors(this, Model.Constructors);
                    @class.AddStubbedMethods(this, Model.Operations);
                },
                baseKind: Model.ParentClass?.GetContractTypeKind());
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