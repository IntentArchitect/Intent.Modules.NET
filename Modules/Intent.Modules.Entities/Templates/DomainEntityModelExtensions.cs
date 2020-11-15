using Intent.Modelers.Domain.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.Entities.Templates
{
    public static class DomainEntityModelExtensions
    {
        public static string Name(this AssociationEndModel associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Name))
            {
                var className = associationEnd.Class.Name;
                return associationEnd.IsCollection ? className.ToPluralName() : className;
            }

            return associationEnd.Name;
        }
    }

    public interface IAttributeTypeConverter
    {
        string ConvertAttributeType(AttributeModel attribute);
    }
}