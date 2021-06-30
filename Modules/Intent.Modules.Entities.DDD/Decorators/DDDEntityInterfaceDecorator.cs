using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.Templates;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityInterface;
using Intent.Modules.Entities.Templates.DomainEntityState;
using AssociationEndModel = Intent.Modelers.Domain.Api.AssociationEndModel;

namespace Intent.Modules.Entities.DDD.Decorators
{
    public class DDDEntityInterfaceDecorator : DomainEntityInterfaceDecoratorBase
    {
        public const string Id = "Intent.Entities.DDD.EntityInterfaceDecorator";

        public DDDEntityInterfaceDecorator(DomainEntityInterfaceTemplate template) : base(template)
        {
        }

        public override string ConvertAttributeType(AttributeModel attribute)
        {
            var domainType = attribute.GetStereotypeProperty<string>("DomainType", "Type");
            if (domainType != null)
            {
                return domainType;
            }
            return base.ConvertAttributeType(attribute);
        }

        public override string AttributeAccessors(AttributeModel attribute)
        {
            return "get;";
        }

        public override bool CanWriteDefaultAssociation(AssociationEndModel association)
        {
            return false;
        }

        public override string PropertyBefore(AssociationEndModel associationEnd)
        {
            if (!associationEnd.IsNavigable)
            {
                return base.PropertyBefore(associationEnd);
            }

            var name = Template.Types.InContext(DomainEntityInterfaceTemplate.InterfaceContext).Get(associationEnd).Name;

            return $@"
        {Template.NormalizeNamespace(name)} {associationEnd.Name().ToPascalCase()} {{ get; }}
";
        }
    }
}
