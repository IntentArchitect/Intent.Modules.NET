﻿using System;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common;
//using Intent.Modules.Metadata.RDBMS.Settings;

namespace Intent.Modules.EntityFrameworkCore.Templates
{
    public static class Extensions
    {
        public static string FindSchema(this ClassModel classModel)
        {
            IHasStereotypes currentElement = classModel.InternalElement;

            if (currentElement.HasStereotype("Table") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("Table", "Schema")?.Trim()))
            {
                return currentElement.GetStereotypeProperty<string>("Table", "Schema")?.Trim();
            }

            if (currentElement.HasStereotype("View") && !string.IsNullOrEmpty(currentElement.GetStereotypeProperty<string>("View", "Schema")?.Trim()))
            {
                return currentElement.GetStereotypeProperty<string>("View", "Schema")?.Trim();
            }

            while (currentElement != null)
            {
                if (currentElement.HasStereotype("Schema"))
                {
                    return currentElement.GetStereotypeProperty<string>("Schema", "Name")?.Trim();
                }

                if (currentElement is not IElement element)
                {
                    break;
                }

                currentElement = element.ParentElement ?? (IHasStereotypes)element.Package;
            }
            return null;
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
