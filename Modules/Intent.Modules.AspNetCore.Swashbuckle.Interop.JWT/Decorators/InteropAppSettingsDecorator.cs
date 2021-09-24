using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Intent.Modules.AspNetCore.Events;
using Intent.Modules.Common.Configuration;
using Intent.Modules.Common.CSharp.Configuration;
using Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Events;
using System;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Swashbuckle.Interop.JWT.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class InteropAppSettingsDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.AspNetCore.Swashbuckle.Interop.JWT.InteropAppSettingsDecorator";

        private readonly AppSettingsTemplate _template;
        private readonly IApplication _application;
        private readonly List<SwaggerOAuth2SchemeEvent> _swaggerSchemes;
        private string _stsPort = SchemeEventConstants.STS_Port_Tag;

        [IntentManaged(Mode.Merge)]
        public InteropAppSettingsDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
            _application.EventDispatcher.Subscribe<SwaggerOAuth2SchemeEvent>(Handle);
            _application.EventDispatcher.Subscribe<SecureTokenServiceHostedEvent>(Handle); // This is just temporary. Need to store these settings in a solution-wide accessible space for each app.
            _swaggerSchemes = new List<SwaggerOAuth2SchemeEvent>();
        }

        private void Handle(SwaggerOAuth2SchemeEvent @event)
        {
            _swaggerSchemes.Add(@event);
        }

        private void Handle(SecureTokenServiceHostedEvent @event)
        {
            _stsPort = @event.Port;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            var settings = appSettings.GetProperty("Swashbuckle");

            dynamic securitySchemes = settings.SwaggerGen.SwaggerGeneratorOptions.SecuritySchemes;
            if (securitySchemes == null)
            {
                securitySchemes = JObject.FromObject(new
                {
                    oauth2 = new
                    {
                        Type = "OAuth2",
                        Flows = new Dictionary<string, object>()
                    }
                });
                settings.SwaggerGen.SwaggerGeneratorOptions.SecuritySchemes = securitySchemes;
            }

            foreach (var scheme in _swaggerSchemes)
            {
                securitySchemes.oauth2.Flows[scheme.SchemeName] = JObject.FromObject(new
                {
                    AuthorizationUrl = scheme.AuthorizationUrl?.Replace(SchemeEventConstants.STS_Port_Tag, _stsPort),
                    RefreshUrl = scheme.RefreshUrl?.Replace(SchemeEventConstants.STS_Port_Tag, _stsPort),
                    TokenUrl = scheme.TokenUrl?.Replace(SchemeEventConstants.STS_Port_Tag, _stsPort),
                    scheme.Scopes
                });
            }

            dynamic oauthConfigObj = settings.SwaggerUI.OAuthConfigObject;
            if (oauthConfigObj == null)
            {
                oauthConfigObj = JObject.FromObject(new
                {
                    ClientId = _swaggerSchemes.OrderByDescending(k => k.Priority).FirstOrDefault()?.ClientId ?? string.Empty,
                    ClientSecret = (string)null,
                    AppName = _template.OutputTarget.Application.Name,
                    UsePkceWithAuthorizationCodeGrant = true,
                    ScopeSeparator = " "
                });
                settings.SwaggerUI.OAuthConfigObject = oauthConfigObj;
            }

            if (_swaggerSchemes.Select(s => s.ClientId).Contains((string)oauthConfigObj.ClientId)
                || oauthConfigObj.ClientId == null
                || oauthConfigObj.ClientId == string.Empty)
            {
                oauthConfigObj.ClientId = _swaggerSchemes.OrderByDescending(k => k.Priority).FirstOrDefault()?.ClientId ?? string.Empty;
            }
        }
    }
}