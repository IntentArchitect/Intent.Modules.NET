using System;
using Intent.Engine;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.EntityFrameworkCore.Templates
{
    public static class Extensions
    {

        public static string GetDefaultSurrogateKeyType(this ISoftwareFactoryExecutionContext executionContext)
        {
            var settingType = executionContext.Settings.GetDatabaseSettings()?.KeyType().Value ?? "guid";
            switch (settingType)
            {
                case "guid":
                    return "System.Guid";
                case "int":
                    return "int";
                case "long":
                    return "long";
                default:
                    return settingType;
            }
        }

        public static string GetDefaultSurrogateKeyType(this ICSharpTemplate template)
        {
            return template.UseType(GetDefaultSurrogateKeyType(template.ExecutionContext));
        }
        public static RelationshipType GetRelationshipType(this AssociationModel association)
        {
            if ((association.SourceEnd.Multiplicity == Multiplicity.One || association.SourceEnd.Multiplicity == Multiplicity.ZeroToOne) && (association.TargetEnd.Multiplicity == Multiplicity.One || association.TargetEnd.Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.OneToOne;
            if ((association.SourceEnd.Multiplicity == Multiplicity.One || association.SourceEnd.Multiplicity == Multiplicity.ZeroToOne) && association.TargetEnd.Multiplicity == Multiplicity.Many)
                return RelationshipType.OneToMany;
            if (association.SourceEnd.Multiplicity == Multiplicity.Many && (association.TargetEnd.Multiplicity == Multiplicity.One || association.TargetEnd.Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.ManyToOne;
            if (association.SourceEnd.Multiplicity == Multiplicity.Many && association.TargetEnd.Multiplicity == Multiplicity.Many)
                return RelationshipType.ManyToMany;

            throw new Exception($"The relationship type from [{association.SourceEnd.Class.Name}] to [{association.TargetEnd.Class.Name}] could not be determined.");
        }

        public static RelationshipType Relationship(this AssociationEndModel associationEnd)
        {
            if ((associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne) && (associationEnd.OtherEnd().Multiplicity == Multiplicity.One || associationEnd.OtherEnd().Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.OneToOne;
            if ((associationEnd.Multiplicity == Multiplicity.One || associationEnd.Multiplicity == Multiplicity.ZeroToOne) && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
                return RelationshipType.OneToMany;
            if (associationEnd.Multiplicity == Multiplicity.Many && (associationEnd.OtherEnd().Multiplicity == Multiplicity.One || associationEnd.OtherEnd().Multiplicity == Multiplicity.ZeroToOne))
                return RelationshipType.ManyToOne;
            if (associationEnd.Multiplicity == Multiplicity.Many && associationEnd.OtherEnd().Multiplicity == Multiplicity.Many)
                return RelationshipType.ManyToMany;

            throw new Exception($"The relationship type from [{associationEnd.Class.Name}] to [{associationEnd.OtherEnd().Class.Name}] could not be determined.");
        }

        public static string MultiplicityString(this AssociationEndModel associationEnd)
        {
            if (associationEnd.IsCollection)
            {
                return associationEnd.IsNullable ? "0..*" : "1..*";
            }
            else
            {
                return associationEnd.IsNullable ? "0..1" : "1";
            }
        }

        public static string RelationshipString(this AssociationModel association)
        {
            return $"{association.SourceEnd.MultiplicityString()}->{association.TargetEnd.MultiplicityString()}";
        }

        public static string IdentifierType(this ClassModel obj)
        {
            return obj.Name.ToPascalCase() + "Id";
        }

        public static string IdentifierName(this AssociationEndModel associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Name))
            {
                return associationEnd.Class.IdentifierType();
            }
            else
            {
                return associationEnd.Name.ToPascalCase() + "Id";
            }
        }
    }

    public enum RelationshipType
    {
        OneToOne,
        OneToMany,
        ManyToOne,
        ManyToMany,
    }
}
