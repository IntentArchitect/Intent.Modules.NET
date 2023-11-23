using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;

namespace Intent.Modules.CosmosDB.Templates
{
    internal static class Helpers
    {
        public const string PrimaryKeyType = "string";
        public const string ValueObjectTypeId = "5fe6bb0a-7fc3-42ae-a351-d9188f5b8bc5";
        public static IEnumerable<IElement> GetValueObjects(this IDesigner designer) => designer.GetElementsOfType(ValueObjectTypeId);
        public static bool CreateEntityInterfaces(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("0456dafe-a46e-466b-bf23-1fb35c094899")?.Value.ToPascalCase(), out var result) && result;
        public static bool EnsurePrivatePropertySetters(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("0cf704e1-9a61-499a-bb91-b20717e334f5")?.Value.ToPascalCase(), out var result) && result;
    }
}
