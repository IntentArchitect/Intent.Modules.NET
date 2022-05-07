using System.ComponentModel;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Engine;

namespace Intent.Modules.Entities.Keys.Decorators
{
    [Description(UnidirectionalManyToManyEntityStateDecorator.Identifier)]
    public class UnidirectionalManyToManyEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate, DomainEntityStateDecoratorBase>
    {
        public override string DecoratorId => UnidirectionalManyToManyEntityStateDecorator.Identifier;

        public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template, IApplication application)
        {
            return new UnidirectionalManyToManyEntityStateDecorator(template);
        }
    }
}
