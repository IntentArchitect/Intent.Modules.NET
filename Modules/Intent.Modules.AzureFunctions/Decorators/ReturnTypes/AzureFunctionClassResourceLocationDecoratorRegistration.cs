using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Decorators.ReturnTypes
{
    [Description(AzureFunctionClassResourceLocationDecorator.DecoratorId)]
    public class AzureFunctionClassResourceLocationDecoratorRegistration : DecoratorRegistration<AzureFunctionClassTemplate, AzureFunctionClassDecorator>
    {
        public override AzureFunctionClassDecorator CreateDecoratorInstance(AzureFunctionClassTemplate template, IApplication application)
        {
            return new AzureFunctionClassResourceLocationDecorator(template, application);
        }

        public override string DecoratorId => AzureFunctionClassResourceLocationDecorator.DecoratorId;
    }
}