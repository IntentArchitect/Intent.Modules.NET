using Intent.Engine;
using Intent.Modelers.Types.ServiceProxies.Api;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates
{
    internal static class ProxySettingsHelper
    {

        internal static bool? GetSerializeEnumsAsStrings(IApplication application, ServiceProxyModel proxy)
        {
            if (!string.IsNullOrEmpty(proxy.InternalElement.MappedElement?.ApplicationId))
            {
                try
                {
                    var settings = application.GetApplicationConfig(proxy.InternalElement.MappedElement.ApplicationId);
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
            }
            return null;
        }

    }
}
