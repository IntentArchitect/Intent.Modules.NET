using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.TemplateDecorator", Version = "1.0")]

namespace Intent.Modules.Security.MSAL.Decorators
{
    [IntentManaged(Mode.Merge)]
    public class ConfigurationSettingsMSALDecorator : AppSettingsDecorator
    {
        [IntentManaged(Mode.Fully)]
        public const string DecoratorId = "Intent.Security.MSAL.ConfigurationSettingsMSALDecorator";

        [IntentManaged(Mode.Fully)]
        private readonly AppSettingsTemplate _template;
        [IntentManaged(Mode.Fully)]
        private readonly IApplication _application;

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public ConfigurationSettingsMSALDecorator(AppSettingsTemplate template, IApplication application)
        {
            _template = template;
            _application = application;
        }

        public override void UpdateSettings(AppSettingsEditor appSettings)
        {
            appSettings.AddPropertyIfNotExists("AzureAd", new
            {
                Instance = "https://login.microsoftonline.com/",
                Domain = "[Enter the domain of your tenant, e.g. contoso.onmicrosoft.com]",
                TenantId = "[Enter 'common', or 'organizations' or the Tenant Id (Obtained from the Azure portal. Select 'Endpoints' from the 'App registrations' blade and use the GUID in any of the URLs), e.g. da41245a5-11b3-996c-00a8-4d99re19f292]",
                ClientId = "[Enter the Client Id (Application ID obtained from the Azure portal), e.g. ba74781c2-53c2-442a-97c2-3d60re42f403]",
                Audience = "api://[Enter the ClientId here]",
                Scopes = "[list of required scopes separated by space \"api://[Client Id here]/App.Read api://[Client Id here]/App.Write\"]",
                CallbackPath = "/signin-oidc",
                SignedOutCallbackPath = "/signout-callback-oidc",
                ClientSecret = "[Copy the client secret added to the app from the Azure portal]"
            });
        }
    }
}