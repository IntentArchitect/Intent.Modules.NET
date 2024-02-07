using System;
using System.Linq;
using Intent.Metadata.DocumentDB.Api;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.Modules.Redis.Om.Repositories.Templates
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

        // Copied from CosmosDB module, removed partition key. Sharding in Redis works differently to Partitions in Document DBs.
        // At time of writing it seems that the way it works is you take the field you wish to partition, use a hashing algorithm,
        // MOD it by the number of available shard instances, pick that URL and connect to that shard instance. Could look at building
        // this later IF necessary (provided someone uses Redis like CosmosDB/MongoDB).
        public record PrimaryKeyData(AttributeModel IdAttribute);

        public static PrimaryKeyData GetPrimaryKeyAttribute(this ClassModel model)
        {
            AttributeModel? idAttribute = null;
            var @class = model;
            while (@class != null)
            {
                var primaryKeyAttributes = @class.Attributes.Where(x => x.HasPrimaryKey()).ToList();
                if (primaryKeyAttributes.Any())
                {
                    foreach (var pk in primaryKeyAttributes)
                    {
                        idAttribute = pk;
                    }
                }

                @class = @class.ParentClass;
            }
            if (idAttribute is null)
            {
                return null;
            }
            return new PrimaryKeyData(IdAttribute: idAttribute);
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
