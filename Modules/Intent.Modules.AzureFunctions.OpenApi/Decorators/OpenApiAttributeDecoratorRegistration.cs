using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.OpenApi.Decorators
{
    [Description(OpenApiAttributeDecorator.DecoratorId)]
    public class OpenApiAttributeDecoratorRegistration : DecoratorRegistration<AzureFunctionClassTemplate, AzureFunctionClassDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override AzureFunctionClassDecorator CreateDecoratorInstance(AzureFunctionClassTemplate template, IApplication application)
        {
            return new OpenApiAttributeDecorator(template, application);
        }

        public override string DecoratorId => OpenApiAttributeDecorator.DecoratorId;
    }
}