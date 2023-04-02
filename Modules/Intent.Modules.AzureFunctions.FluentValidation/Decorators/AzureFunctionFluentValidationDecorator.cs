using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.AzureFunctions.FluentValidation.Templates;
using Intent.Modules.AzureFunctions.FluentValidation.Templates.ValidationServiceInterface;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.FluentValidation.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class AzureFunctionFluentValidationDecorator : AzureFunctionClassDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AzureFunctions.FluentValidation.AzureFunctionFluentValidationDecorator";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public AzureFunctionFluentValidationDecorator(AzureFunctionClassTemplate template, IApplication application)
        {
            var requestDtoTypeName = template.Model.GetRequestDtoParameter() != null
                ? template.GetTypeName(template.Model.GetRequestDtoParameter().TypeReference)
            : null;

            if (requestDtoTypeName == null || template.Model.GetAzureFunction()?.Type().IsHttpTrigger() != true ||
                template.Model.Mapping?.Element?.AsOperationModel() == null)
            {
                return;
            }

            template.AddNugetDependency(NuGetPackages.FluentValidation);

            template.CSharpFile.OnBuild(file =>
            {
                file.AddUsing("FluentValidation");
                var @class = file.Classes.Single();
                @class.Constructors.First().AddParameter(template.GetValidationServiceInterfaceName(), "validator", param =>
                    {
                        param.IntroduceReadonlyField((_, assignment) => assignment.ThrowArgumentNullException());
                    });

                var runMethod = @class.FindMethod("Run");
                runMethod.FindStatement(x => x.HasMetadata("service-dispatch-statement"))
                    ?.InsertAbove($"await _validator.Validate({template.Model.GetRequestDtoParameter().Name.ToParameterName()}, default);");

                runMethod.FindStatement<CSharpTryBlock>(x => true)
                    ?.InsertBelow(new CSharpCatchBlock("ValidationException", "exception")
                        .AddStatement("return new BadRequestObjectResult(exception.Errors);"));

            }, 10);
        }
    }
}