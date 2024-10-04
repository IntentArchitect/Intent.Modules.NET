using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Blazor.FluentValidation.Templates.ValidatorProviderInterface
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class ValidatorProviderInterfaceTemplate : CSharpTemplateBase<object>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Blazor.FluentValidation.ValidatorProviderInterface";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ValidatorProviderInterfaceTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.FluentValidation(outputTarget));

            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("FluentValidation")
                .AddInterface($"IValidatorProvider", inter =>
                {
                    inter.AddMethod($"IValidator<T>", "GetValidator", method => method.AddGenericParameter("T"));
                });
        }

        // public override bool CanRunTemplate()
        // {
        //     var validatorTemplates = ExecutionContext.FindTemplateInstances<ICSharpFileBuilderTemplate>(DtoValidatorTemplate.TemplateId);
        //     return validatorTemplates.Any(p => p.CSharpFile.Classes.FirstOrDefault()?.Constructors.FirstOrDefault()?.Parameters.Any(q => q.Type.Contains("IValidatorProvider")) == true);
        // }

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