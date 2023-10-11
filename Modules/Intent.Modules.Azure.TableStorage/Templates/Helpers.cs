using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Azure.TableStorage.Templates
{
    internal static class Helpers
    {
        public const string ValueObjectTypeId = "5fe6bb0a-7fc3-42ae-a351-d9188f5b8bc5";
        public static IEnumerable<IElement> GetValueObjects(this IDesigner designer) => designer.GetElementsOfType(ValueObjectTypeId);
        public static bool EnsurePrivatePropertySetters(this DomainSettings groupSettings) => bool.TryParse(groupSettings.GetSetting("0cf704e1-9a61-499a-bb91-b20717e334f5")?.Value.ToPascalCase(), out var result) && result;
    }

}
