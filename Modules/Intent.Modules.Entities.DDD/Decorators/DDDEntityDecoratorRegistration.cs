using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntity;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityDecorator.Identifier)]
    public class DDDEntityDecoratorRegistration : DecoratorRegistration<DomainEntityTemplate, DomainEntityDecoratorBase>
    {
        public override string DecoratorId => DDDEntityDecorator.Identifier;

        public override DomainEntityDecoratorBase CreateDecoratorInstance(DomainEntityTemplate template, IApplication application)
        {
            return new DDDEntityDecorator(template);
        }
    }
}
