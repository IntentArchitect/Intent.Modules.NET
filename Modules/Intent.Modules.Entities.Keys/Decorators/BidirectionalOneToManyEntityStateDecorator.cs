using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Configuration;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Plugins;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class BidirectionalOneToManyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        public const string Identifier = "Intent.Entities.Keys.BidirectionalOneToManyEntityDecorator";

        public BidirectionalOneToManyEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
        }

        public override string AssociationBefore(AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsNavigable && associationEnd.Multiplicity == Multiplicity.Many && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
            {
                return $@"       protected virtual { Template.GetTypeName(associationEnd) } { associationEnd.Name().ToPascalCase() } {{ get; set; }}
";
            }
            return base.AssociationBefore(associationEnd);
        }
    }
}
