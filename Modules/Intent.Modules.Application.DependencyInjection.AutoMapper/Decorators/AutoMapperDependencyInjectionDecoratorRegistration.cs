using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.DependencyInjection.Templates.DependencyInjection;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.AutoMapper.Decorators
{
    [Description(AutoMapperDependencyInjectionDecorator.DecoratorId)]
    public class AutoMapperDependencyInjectionDecoratorRegistration : DecoratorRegistration<DependencyInjectionTemplate, DependencyInjectionDecorator>
    {
        [IntentManaged(Mode.Fully)]
        public override DependencyInjectionDecorator CreateDecoratorInstance(DependencyInjectionTemplate template, IApplication application)
        {
            return new AutoMapperDependencyInjectionDecorator(template, application);
        }

        public override string DecoratorId => AutoMapperDependencyInjectionDecorator.DecoratorId;
    }
}