using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(SurrogatePrimaryKeyEntityStateDecorator.Identifier)]
    public class SurrogatePrimaryKeyEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => SurrogatePrimaryKeyEntityStateDecorator.Identifier;
        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new SurrogatePrimaryKeyEntityStateDecorator(template);
        }
    }
}
