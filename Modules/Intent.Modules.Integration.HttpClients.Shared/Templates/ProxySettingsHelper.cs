using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates
{
    public static class ProxySettingsHelper
    {
        public static bool SerializeEnumsAsStrings(this IIntentTemplate template, IElement? element)
        {
            var applicationId = element?.MappedElement?.Element?.Package.ApplicationId ?? element?.Package.ApplicationId;
            if (applicationId == null)
            {
                return false;
            }

            try
            {
                var settings = template.ExecutionContext.GetApplicationConfig(applicationId);
                //API Settings
                var apiSettings = settings.ModuleSetting.FirstOrDefault(s => s.Id == "4bd0b4e9-7b53-42a9-bb4a-277abb92a0eb");
                //SerializeEnumsAsStrings
                if (bool.TryParse(apiSettings?.GetSetting("97f3a1e3-2455-41e8-b28e-709f2db04230")?.Value?.ToPascalCase(), out var result))
                {
                    return result;
                }
            }
            catch // This is if the applicationId can't be found need a TryGetApplicationConfig, will be added 4.3
            {
                return false;
            }

            return false;
        }
    }
}
