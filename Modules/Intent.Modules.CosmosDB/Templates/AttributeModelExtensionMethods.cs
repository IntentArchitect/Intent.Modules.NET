using System;
using System.Linq;
using System.Reflection.Metadata;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.CosmosDB.Templates
{
    internal static class AttributeModelExtensionMethods
    {
        public static string GetToString<T>(this AttributeModel attribute, CSharpTemplateBase<T> template)
        {
            return attribute.TypeReference.Element?.Name.ToLowerInvariant() switch
            {
                "string" => string.Empty,
                "long" or "int" => $".ToString({template.UseType("System.Globalization.CultureInfo")}.InvariantCulture)",
                _ => ".ToString()"
            };
        }

        public static string SetToType<T>(this AttributeModel attribute, CSharpTemplateBase<T> template)
        {
            string csharpType = template.GetTypeName(attribute.TypeReference);
            return attribute.TypeReference.Element?.Name.ToLowerInvariant() switch
            {
                "string" => "value!",
                _ => $"({csharpType})Convert.ChangeType(value, typeof({csharpType}))!"
            };
        }


        public static AttributeModel GetPrimaryKeyAttribute(this ClassModel model)
        {
            var @class = model;
            while (@class != null)
            {
                var primaryKeyAttribute = @class.Attributes.SingleOrDefault(x => x.HasPrimaryKey());
                if (primaryKeyAttribute != null)
                {
                    return primaryKeyAttribute;
                }

                @class = @class.ParentClass;
            }

            return null;
        }

        public static AttributeModel GetAttributeOrDerivedWithName(this ClassModel model, string name)
        {
            var @class = model;
            while (@class != null)
            {
                var primaryKeyAttribute = @class.Attributes.SingleOrDefault(x => name.Equals(x.Name, StringComparison.OrdinalIgnoreCase));
                if (primaryKeyAttribute != null)
                {
                    return primaryKeyAttribute;
                }

                @class = @class.ParentClass;
            }

            return null;
        }
    }
}
