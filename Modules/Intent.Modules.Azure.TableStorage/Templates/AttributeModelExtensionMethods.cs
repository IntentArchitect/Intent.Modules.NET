using Intent.Modelers.Domain.Api;
using Intent.Modules.Common.CSharp.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.DocumentDB.Api;

namespace Intent.Modules.Azure.TableStorage.Templates
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

        public static IEnumerable<AttributeModel> GetPrimaryKeyAttribute(this ClassModel model)
        {
            var @class = model;
            while (@class != null)
            {
                var primaryKeyAttribute = @class.Attributes.Where(x => x.HasPrimaryKey());
                if (primaryKeyAttribute.Any())
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
