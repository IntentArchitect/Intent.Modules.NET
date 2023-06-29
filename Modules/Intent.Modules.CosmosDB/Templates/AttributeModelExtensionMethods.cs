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
    }
}
