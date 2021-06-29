using System.ComponentModel;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.DDD.Decorators
{
    [Description(DDDEntityStateDecorator.Identifier)]
    public class DDDEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => DDDEntityStateDecorator.Identifier;

        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new DDDEntityStateDecorator(template);
        }
    }
}
