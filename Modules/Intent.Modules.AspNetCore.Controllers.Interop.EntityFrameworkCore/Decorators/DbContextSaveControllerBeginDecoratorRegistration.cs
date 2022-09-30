using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.AspNetCore.Controllers.Templates.Controller;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Controllers.Interop.EntityFrameworkCore.Decorators
{
    [Description(DbContextSaveControllerBeginDecorator.DecoratorId)]
    public class DbContextSaveControllerBeginDecoratorRegistration : DecoratorRegistration<ControllerTemplate, ControllerDecorator>
    {
        public override ControllerDecorator CreateDecoratorInstance(ControllerTemplate template, IApplication application)
        {
            return new DbContextSaveControllerBeginDecorator(template, application);
        }

        public override string DecoratorId => DbContextSaveControllerBeginDecorator.DecoratorId;
    }
}