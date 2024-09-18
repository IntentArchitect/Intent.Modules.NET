using System;
using System.Linq;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBDocumentTypeExtensionMethods", Version = "1.0")]

namespace CosmosDB.EnumStrings.Infrastructure.Persistence.Documents
{
    internal static class TypeExtensionMethods
    {
        public static string GetNameForDocument(this Type type)
        {
            if (type.IsArray)
            {
                return GetNameForDocument(type.GetElementType()!) + "[]";
            }

            if (type.IsGenericType)
            {
                return $"{type.Name[..type.Name.LastIndexOf("`", StringComparison.InvariantCulture)]}<{string.Join(", ", type.GetGenericArguments().Select(GetNameForDocument))}>";
            }

            return type.Name;
        }
    }
}