using System.Linq;
using Intent.AzureFunctions.Api;
using Intent.Engine;
using Intent.Modules.Application.FluentValidation.Dtos.Templates;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Builder;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.RoslynWeaver.Attributes;

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
                var requestDtoTypeName = template.Model.GetRequestDtoParameter() != null
                    ? template.GetTypeName(template.Model.GetRequestDtoParameter().TypeReference)
                    : null;

                if (requestDtoTypeName == null || template.Model.TriggerType != TriggerType.HttpTrigger)
                {
                    continue;
                }

                template.AddNugetDependency(NugetPackages.FluentValidation(template.OutputTarget));

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
                        dispatchStatement.InsertAbove($"await _validator.Handle({template.Model.GetRequestDtoParameter().Name.ToParameterName()}, cancellationToken);");
                    }

                    runMethod.FindStatement<CSharpTryBlock>(x => true)
                        ?.InsertBelow(new CSharpCatchBlock("ValidationException", "exception")
                            .AddStatement("return new BadRequestObjectResult(exception.Errors);"));

                }, 10);
            }
        }
    }
}