using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Services.Api;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Application.FluentValidation.Dtos.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AzureFunctionFluentValidationDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.FluentValidation.AzureFunctionFluentValidationDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AzureFunctionClassTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionFluentValidationDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            var requestDtoTypeName = template.Model.GetRequestDtoParameter() != null
                ? template.GetTypeName(template.Model.GetRequestDtoParameter().TypeReference)
            : null;

            if (requestDtoTypeName == null || template.Model.TriggerType != TriggerType.HttpTrigger)
            {
                return;
            }

            template.AddNugetDependency(NuGetPackages.FluentValidation);

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("FluentValidation");
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
                    dispatchStatement.InsertAbove($"await _validator.Handle({template.Model.GetRequestDtoParameter().Name.ToParameterName()}, default);");
                }

                runMethod.FindStatement<CSharpTryBlock>(x => true)
                    ?.InsertBelow(new CSharpCatchBlock("ValidationException", "exception")
                        .AddStatement("return new BadRequestObjectResult(exception.Errors);"));

            }, 10);
        }
    }
}