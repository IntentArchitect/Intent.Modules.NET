using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Application.Dtos.Templates.DtoModel;
using Intent.Modules.Common.Registrations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.AutoMapper.Decorators
{
    [Description(AutoMapperDtoDecorator.DecoratorId)]
    public class AutoMapperDtoDecoratorRegistration : DecoratorRegistration<DtoModelTemplate, DtoModelDecorator>
    {
        public override DtoModelDecorator CreateDecoratorInstance(DtoModelTemplate template, IApplication application)
        {
            return new AutoMapperDtoDecorator(template);
        }

        public override string DecoratorId => AutoMapperDtoDecorator.DecoratorId;
    }
}