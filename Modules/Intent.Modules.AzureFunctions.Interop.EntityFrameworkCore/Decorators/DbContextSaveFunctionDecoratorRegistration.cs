using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AzureFunctions.Templates.AzureFunctionClass;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AzureFunctions.Interop.EntityFrameworkCore.Decorators
{
    [Description(DbContextSaveFunctionDecorator.DecoratorId)]
    public class DbContextSaveFunctionDecoratorRegistration : DecoratorRegistration<AzureFunctionClassTemplate, AzureFunctionClassDecorator>
    {
        public override AzureFunctionClassDecorator CreateDecoratorInstance(AzureFunctionClassTemplate template, IApplication application)
        {
            return new DbContextSaveFunctionDecorator(template, application);
        }

        public override string DecoratorId => DbContextSaveFunctionDecorator.DecoratorId;
    }
}