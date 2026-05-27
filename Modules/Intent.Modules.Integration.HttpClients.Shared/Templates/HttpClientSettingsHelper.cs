using Intent.Modules.Common.Templates;

namespace Intent.Modules.Integration.HttpClients.Shared.Templates
{
    public enum HttpClientSettingScope
    {
        Group,
        Service
    }

    public static class HttpClientSettingsHelper
    {
        public const string HttpClientsSection = "HttpClients";

        public static string GetConfigKey(this IServiceProxyModel proxy, HttpClientSettingScope settingScope, string key)
        {
            switch (settingScope)
            {
                case HttpClientSettingScope.Group:
                    return GetConfigKey(proxy.GetGroupName(), key);
                case HttpClientSettingScope.Service:
                default:
                    return GetConfigKey(proxy.Name.ToPascalCase(), key);
            }
        }

        public static string GetConfigKey(string groupName, string key)
        {
            return $"{HttpClientsSection}:{groupName}:{key.ToPascalCase()}";
        }
    }
}
