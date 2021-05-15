using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Configuration;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Plugins;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class ForeignKeyEntityStateDecorator : DomainEntityStateDecoratorBase, ISupportsConfiguration
    {
        private string _foreignKeyType = "Guid";
        public const string Identifier = "Intent.Entities.Keys.ForeignKeyEntityDecorator";
        public const string ForeignKeyType = "Foreign Key Type";

        public ForeignKeyEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
            Priority = -100;
        }

        public override string AssociationBefore(AssociationEndModel associationEnd)
        {
            if (associationEnd.RequiresForeignKey())
            {
                if (associationEnd.OtherEnd().Element.HasStereotype("Foreign Key"))
                {
                    return base.AssociationBefore(associationEnd);
                }
                var foreignKeyType = associationEnd.Class.GetSurrogateKeyType(Template.Types);
                return $@"       public {foreignKeyType}{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name().ToPascalCase() }Id {{ get; set; }}
";
            }
            return base.AssociationBefore(associationEnd);
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            if (settings.ContainsKey(ForeignKeyType) && !string.IsNullOrWhiteSpace(settings[ForeignKeyType]))
            {
                _foreignKeyType = settings[ForeignKeyType];
            }
        }
    }
}
