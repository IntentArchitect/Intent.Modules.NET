using System;
using System.Linq;
using System.Reflection;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Application.FluentValidation.Dtos.Templates;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.FactoryExtension", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.FactoryExtensions
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FluentValidationExtension : FactoryExtensionBase
    {
        public override string Id => "Intent.AzureFunctions.FluentValidation.FluentValidationExtension";

        [IntentManaged(Mode.Ignore)]
        public override int Order => 0;

        protected override void OnAfterTemplateRegistrations(IApplication application)
        {
            var templates = application.FindTemplateInstances<AzureFunctionClassTemplate>(TemplateDependency.OnTemplate(AzureFunctionClassTemplate.TemplateId));
            foreach (var template in templates)
            {
                var validator = TryGetValidator(template);

                if (validator == null || template.Model.TriggerType != TriggerType.HttpTrigger)
                {
                    continue;
                }

                template.AddNugetDependency(NugetPackages.FluentValidation(template.OutputTarget));

                template.CSharpFile.OnBuild(file =>
                {
                    var @class = file.Classes.Single();

                    var runMethod = @class.FindMethod("Run");

                    var dispatchStatement = runMethod.FindStatement(x => x.HasMetadata("service-dispatch-statement"));
                    if (dispatchStatement != null)
                    {
                        @class.Constructors.First()
                            .AddParameter(
                                type: template.GetValidationServiceInterfaceName(),
                                name: "validator",
                                configure: param => param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException()));
                        dispatchStatement.InsertAbove($"await _validator.Handle({template.Model.GetRequestDtoParameter().Name.ToParameterName()}, cancellationToken);");

                    }

                    runMethod.FindStatement<CSharpTryBlock>(x => true)?
                        .InsertBelow(new CSharpCatchBlock(template.UseType("FluentValidation.ValidationException"), "exception")
                        .AddStatement("return new BadRequestObjectResult(exception.Errors);"));

                }, 10);
            }
        }

        private ICSharpFileBuilderTemplate TryGetValidator(AzureFunctionClassTemplate template)
        {
            if (!TryGetTemplate<ICSharpFileBuilderTemplate>(template, TemplateRoles.Application.Validation.Command, template.Model.InternalElement, t => t.CanRunTemplate(), out var result)
                && !TryGetTemplate(template, TemplateRoles.Application.Validation.Query, template.Model.InternalElement, t => t.CanRunTemplate(), out result))
            {
                var requestDtoTypeName = template.Model.GetRequestDtoParameter();
                if (requestDtoTypeName is not null)
                {
                    TryGetTemplate(template, TemplateRoles.Application.Validation.Dto, requestDtoTypeName.TypeReference.Element, t => t.CanRunTemplate(), out result);
                }
            }
            return result;
        }

        private bool TryGetTemplate<T>(
            AzureFunctionClassTemplate template,
            string templateId,
            IMetadataModel model,
            Func<ITemplate, bool> predicate,
            out T result)
            where T : class, ITemplate
        {
            var templates = template.ExecutionContext.FindTemplateInstances(templateId, model.Id);

            var filtered = predicate is null ? templates : templates.Where(predicate);

            result = filtered.OfType<T>().FirstOrDefault();

            return result is not null;
        }
    }
}