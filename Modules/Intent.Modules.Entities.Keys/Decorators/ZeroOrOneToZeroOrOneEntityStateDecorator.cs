using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Modules.Entities.Templates;

namespace Intent.Modules.Entities.Keys.Decorators;

public class ZeroOrOneToZeroOrOneEntityStateDecorator : DomainEntityStateDecoratorBase
{
    public const string Identifier = "Intent.Entities.Keys.ZeroOrOneToZeroOrOneEntityStateDecorator";

    public ZeroOrOneToZeroOrOneEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
    {
    }

    public override string AssociationAfter(AssociationEndModel associationEnd)
    {
        if (associationEnd.IsTargetEnd()
            && associationEnd.IsNullable && !associationEnd.IsCollection
            && associationEnd.OtherEnd().IsNullable && !associationEnd.OtherEnd().IsCollection)
        {
            return $@"       public Guid? {associationEnd.Name().ToPascalCase()}Id {{ get; set; }}";
        }

        return base.AssociationAfter(associationEnd);
    }
}