using System;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Entities.Templates;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.Metadata.RDBMS.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.Entities.Keys.Decorators
{
    public class ForeignKeyEntityStateDecorator : DomainEntityStateDecoratorBase
    {
        private string _implicitForeignKeyType;
        public const string Identifier = "Intent.Entities.Keys.ForeignKeyEntityDecorator";
        public const string ForeignKeyType = "Foreign Key Type";

        public ForeignKeyEntityStateDecorator(DomainEntityStateTemplate template) : base(template)
        {
            Priority = -100;
            _implicitForeignKeyType = template.ExecutionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "System.Guid";
        }

        public override string AssociationBefore(AssociationEndModel associationEnd)
        {
            if (associationEnd.RequiresForeignKey())
            {
                if (associationEnd.OtherEnd().Element.HasStereotype("Foreign Key"))
                {
                    return base.AssociationBefore(associationEnd);
                }

                if (associationEnd.Class.GetExplicitPrimaryKey().Any())
                {
                    return string.Join(Environment.NewLine, associationEnd.Class.GetExplicitPrimaryKey()
                        .Where(x => associationEnd.OtherEnd().Class.Attributes.All(a => !a.Name.Equals($"{ associationEnd.Name().ToPascalCase() }{x.Name.ToPascalCase()}")))
                        .Select(x => $"       public {Template.GetTypeName(x.TypeReference)}{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name().ToPascalCase() }{x.Name.ToPascalCase()} {{ get; set; }}"));
                }
                else
                {
                    return $@"
       public {Template.UseType(_implicitForeignKeyType)}{ (associationEnd.IsNullable ? "?" : "") } { associationEnd.Name().ToPascalCase() }Id {{ get; set; }}";
                }
            }
            return base.AssociationBefore(associationEnd);
        }

        //public override void Configure(IDictionary<string, string> settings)
        //{
        //    base.Configure(settings);
        //    if (settings.ContainsKey(ForeignKeyType) && !string.IsNullOrWhiteSpace(settings[ForeignKeyType]))
        //    {
        //        _foreignKeyType = settings[ForeignKeyType];
        //    }
        //}
    }
}
