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
            CSharpFile = new CSharpFile(this.GetNamespace(), this.GetFolderPath())
                .AddUsing("System")
                .AddUsing("FluentValidation")
                .AddClass($"{Model.Name}Validator", @class =>
                {
                    @class.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                    @class.WithBaseType($"AbstractValidator<{GetDtoModel()}>");
                    @class.AddConstructor(ctor =>
                    {
                        ctor.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored().WithSignatureMerge());
                        ctor.AddStatement("ConfigureValidationRules();");
                    });
                    @class.AddMethod("void", "ConfigureValidationRules", method =>
                    {
                        method.Private();
                        method.AddAttribute(CSharpIntentManagedAttribute.Fully());
                        method.AddStatements(this.GetValidationRulesStatements(Model.Fields));
                    });
                    AddCustomValidationMethods(@class);
                });
        }

        private void AddCustomValidationMethods(CSharpClass @class)
        {
            foreach (var property in Model.Fields)
            {
                if (property.HasValidations() && property.GetValidations().HasCustomValidation())
                {
                    CSharpFile.AddUsing("System.Threading");
                    CSharpFile.AddUsing("System.Threading.Tasks");
                    @class.AddMethod("Task<bool>", $"Validate{property.Name.ToPascalCase()}Async", method =>
                    {
                        method.Private().Async();
                        method.AddAttribute(CSharpIntentManagedAttribute.Merge().WithSignatureFully());
                        method.AddParameter(GetDtoModel(), "model");
                        method.AddParameter(GetTypeName(property), "value");
                        method.AddParameter("CancellationToken", "cancellationToken");
                        method.AddStatement($@"throw new NotImplementedException(""Your custom validation rules here..."");");
                    });
                }
            }
        }

        private string GetDtoModel()
        {
            return GetTypeName(DtoModelTemplate.TemplateId, Model);
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