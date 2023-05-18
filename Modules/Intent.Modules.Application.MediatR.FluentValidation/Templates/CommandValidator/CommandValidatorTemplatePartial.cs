using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Application.FluentValidation.Api;
using Intent.Engine;
using Intent.Modelers.Services.CQRS.Api;
using Intent.Modules.Application.FluentValidation.Templates;
using Intent.Modules.Application.MediatR.Templates.CommandModels;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Application.MediatR.FluentValidation.Templates.CommandValidator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public partial class CommandValidatorTemplate : CSharpTemplateBase<CommandModel>, ICSharpFileBuilderTemplate
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.Application.MediatR.FluentValidation.CommandValidator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public CommandValidatorTemplate(IOutputTarget outputTarget, CommandModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NuGetPackages.FluentValidation);

            var csharpFile = new CSharpFile(this.GetNamespace(additionalFolders: Model.GetConceptName()), this.GetFolderPath(additionalFolders: Model.GetConceptName()));
            csharpFile.AddClass($"{Model.Name}Validator");
            csharpFile.OnBuild((Action<CSharpFile>)(file =>
            {
                file.AddUsing("System");
                file.AddUsing("FluentValidation");

                var @class = file.Classes.First();
                @class.WithBaseType($"AbstractValidator <{GetCommandModel()}>");

                @class.AddConstructor(ctor =>
                {
                    ctor.AddStatement("ConfigureValidationRules();");
                    ctor.AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored().WithSignatureMerge());

                });

                @class.AddMethod("void", "ConfigureValidationRules", method =>
                {
                    method.Private();
                    method.AddAttribute(CSharpIntentManagedAttribute.Fully());

                    foreach (var propertyStatement in this.GetValidationRulesStatements(Model.Properties))
                    {
                        method.AddStatement(propertyStatement);
                    }
                });

                foreach (var property in Model.Properties)
                {
                    if (property.HasValidations() && property.GetValidations().HasCustomValidation())
                    {
                        file.AddUsing("System.Threading");
                        file.AddUsing("System.Threading.Tasks");

                        @class.AddMethod("Task<bool>", $"Validate{property.Name}Async", method =>
                        {
                            method
                                .AddAttribute(CSharpIntentManagedAttribute.Fully().WithBodyIgnored())
                                .Private()
                                .Async();
                            method.AddParameter(GetCommandModel(), "command");
                            method.AddParameter(GetTypeName(property), "value");
                            method.AddParameter("CancellationToken", "cancellationToken");
                            method.AddStatement("throw new NotImplementedException(\"our custom validation rules here...\");");
                        });
                    }
                }
            }));
            CSharpFile = csharpFile;
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

        private string GetCommandModel()
        {
            return GetTypeName(CommandModelsTemplate.TemplateId, Model);
        }
    }
}