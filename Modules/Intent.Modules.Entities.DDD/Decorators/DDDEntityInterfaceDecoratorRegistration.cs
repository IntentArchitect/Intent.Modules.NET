using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityInterface;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityInterfaceDecorator.Id)]
    public class DDDEntityInterfaceDecoratorRegistration : DecoratorRegistration<DomainEntityInterfaceTemplate, DomainEntityInterfaceDecoratorBase>
    {
        public override string DecoratorId => DDDEntityInterfaceDecorator.Id;

        public override DomainEntityInterfaceDecoratorBase CreateDecoratorInstance(DomainEntityInterfaceTemplate template, IApplication application)
        {
            return new DDDEntityInterfaceDecorator(template);
        }
    }
}
