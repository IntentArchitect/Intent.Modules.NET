using System;
using System.Collections.Generic;
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

        public record PrimaryKeyData(AttributeModel IdAttribute, AttributeModel PartitionKeyAttribute);

        public static PrimaryKeyData GetPrimaryKeyAttribute(this ClassModel model)
        {
            model.TryGetPartitionKeySettings(out var partitionKey);
            AttributeModel? idAttribute = null;
            AttributeModel? partitionKeyAttribute = null;
            var @class = model;
            while (@class != null)
            {
                var primaryKeyAttributes = @class.Attributes.Where(x => x.HasPrimaryKey()).ToList();
                if (primaryKeyAttributes.Any())
                {
                    foreach (var pk in primaryKeyAttributes)
                    {
                        if (partitionKey != null && pk.Name.Equals(partitionKey, StringComparison.OrdinalIgnoreCase))
                        {
                            partitionKeyAttribute = pk;
                        }
                        else
                        {
                            idAttribute = pk;
                        }
                    }
                }

                @class = @class.ParentClass;
            }
            if (idAttribute is null && partitionKey is null)
            {
                return null;
            }
            return new PrimaryKeyData(IdAttribute: idAttribute ?? partitionKeyAttribute, PartitionKeyAttribute: partitionKeyAttribute ?? idAttribute);
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
