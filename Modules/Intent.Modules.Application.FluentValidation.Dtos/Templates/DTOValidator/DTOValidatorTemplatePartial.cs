using System.Collections.Generic;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Application.FluentValidation.Templates;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.FluentValidation.Dtos.Templates.DTOValidator
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public partial class DTOValidatorTemplate : CSharpTemplateBase<DTOModel>, ICSharpFileBuilderTemplate
    {
        public const string TemplateId = "Intent.Application.FluentValidation.Dtos.DTOValidator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public DTOValidatorTemplate(IOutputTarget outputTarget, DTOModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.FluentValidation);

            CSharpFile = new CSharpFile(
                    this.GetNamespace(),
                    this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("FluentValidation")
                .AddClass($"{Model.Name}Validator", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());

                    this.ConfigureForValidation(
                        @class: @class,
                        properties: Model.Fields,
                        modelTypeName: GetTypeName(DtoModelTemplate.TemplateId, Model),
                        modelParameterName: "model");
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