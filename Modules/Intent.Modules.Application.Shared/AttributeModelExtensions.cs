﻿using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Shared
{
    /// <summary>
    /// The purpose of these extensions is to abstract how we determine whether an attribute is a Primary Key, Foreign Key, etc.
    /// This was created to decouple this module from Intent.Modules.Metdata.
    /// </summary>
    internal static class AttributeModelExtensions
    {
        public static bool IsPrimaryKey(this AttributeModel attribute)
        {
            return attribute.HasStereotype("Primary Key");
        }

        public static bool IsForeignKey(this AttributeModel attribute)
        {
            return attribute.HasStereotype("Foreign Key");
        }

        public static AssociationTargetEndModel GetForeignKeyAssociation(this AttributeModel attribute)
        {
            return attribute.GetStereotype("Foreign Key")?.GetProperty<IElement>("Association")?.AsAssociationTargetEndModel();
        }
    }
}