using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Entities.Templates.DomainEntityState;

namespace Intent.Modules.Entities.Keys.Decorators;

public class
    ZeroOrOneToZeroOrOneEntityStateDecoratorRegistration : DecoratorRegistration<DomainEntityStateTemplate,
        DomainEntityStateDecoratorBase>
{
    public override DomainEntityStateDecoratorBase CreateDecoratorInstance(DomainEntityStateTemplate template,
        IApplication application)
    {
        return new ZeroOrOneToZeroOrOneEntityStateDecorator(template);
    }

    public override string DecoratorId => ZeroOrOneToZeroOrOneEntityStateDecorator.Identifier;
}