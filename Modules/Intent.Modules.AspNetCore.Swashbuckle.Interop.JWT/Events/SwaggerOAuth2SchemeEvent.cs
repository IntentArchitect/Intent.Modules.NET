using System;
using System.Collections.Generic;

namespace Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events
{
    public static class SchemeEventConstants
    {
        public const string STS_Port_Tag = "{sts_port}";
    }

    public class SwaggerOAuth2SchemeEvent
    {
        public SwaggerOAuth2SchemeEvent(
            string schemeName,
            int priority,
            string clientId,
            string authUrl,
            string tokenUrl,
            string refreshUrl = null,
            Dictionary<string, string> scopes = null)
        {
            SchemeName = schemeName;
            Priority = priority;
            ClientId = clientId;
            AuthorizationUrl = authUrl;
            TokenUrl = tokenUrl;
            RefreshUrl = refreshUrl;
            Scopes = scopes;
        }

        public string SchemeName { get; }
        public int Priority { get; }
        public string ClientId { get; }
        public string AuthorizationUrl { get; }
        public string TokenUrl { get; }
        public string RefreshUrl { get; }
        public Dictionary<string, string> Scopes { get; }
    }
}
