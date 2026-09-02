using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Blazor.Authentication.Api.UserInterfacePackageModelStereotypeExtensions.Security;

namespace Intent.Modules.Blazor.Authentication.Api;

public static class SecurityHelperExtensions
{
    // Element metadata key stamped by the "Security Type" stereotype's page-tagging script onto the
    // modelled Login/Register/etc. Component elements it creates. Same key as
    // AuthPageDefaultContentFactoryExtension's PageIdMetadataKey.
    private const string AuthPageIdMetadataKey = "blazor-auth-page-id";

    // The page-tagging script prefixes the login page's id per Authentication mode. Matched as an
    // explicit set rather than an "ends with -login" test, because "external-login" would also match
    // that and is a different page.
    private static readonly string[] LoginPageIds = ["identity-login", "jwt-login", "oidc-login"];

    // What the page-tagging script assigns as the login page's Page.Route in every mode. Used when no
    // tagged login page can be found in the model (e.g. Authentication = None, or the seeded pages
    // were deleted), so generated redirects still point somewhere sane.
    private const string DefaultLoginRoute = "/Account/Login";

    public static AuthenticationOptions GetAuthenticationType(this IMetadataManager metadataManager, string applicationId)
    {
        var uiDesigner = metadataManager.GetDesigner(applicationId, Designers.UserInterface);

        if (uiDesigner == null)
        {
            return new AuthenticationOptions(AuthenticationOptionsEnum.None.ToString());
        }

        var package = uiDesigner.GetUserInterfacePackageModels().FirstOrDefault(p => p.HasSecurity());

        if (package is null)
        {
            return new AuthenticationOptions(AuthenticationOptionsEnum.None.ToString());
        }

        return package.GetSecurity().Authentication();
    }

    public static AuthenticationOptions GetAuthenticationType(this RazorComponentTemplateBase<ComponentModel> template)
    {
        var model = template.Model;

        if(model is null)
        {
            return new AuthenticationOptions(AuthenticationOptionsEnum.None.ToString());
        }

        var package = model.InternalElement.Package;
        if(package is null || !package.IsUserInterfacePackageModel())
        {
            return new AuthenticationOptions(AuthenticationOptionsEnum.None.ToString());
        }

        var uiPackage = new UserInterfacePackageModel(package);
        if(!uiPackage.HasSecurity())
        {
            return new AuthenticationOptions(AuthenticationOptionsEnum.None.ToString());
        }

        return uiPackage.GetSecurity().Authentication();
    }

    /// <summary>
    /// The authentication mode for the application this template is generating into.
    /// </summary>
    public static AuthenticationOptions GetAuthenticationType(this IIntentTemplate template)
    {
        return template.ExecutionContext.MetadataManager.GetAuthenticationType(
            template.ExecutionContext.GetApplicationConfig().Id);
    }

    /// <summary>
    /// The route of the modelled login page, read from its Page stereotype so that a user who edits
    /// the route in the User Interface designer has every generated redirect follow them. Falls back
    /// to <c>/Account/Login</c> when no tagged login page is in the model.
    /// </summary>
    public static string GetLoginRoute(this IIntentTemplate template)
    {
        return template.ExecutionContext.MetadataManager.GetLoginRoute(
            template.ExecutionContext.GetApplicationConfig().Id);
    }

    /// <inheritdoc cref="GetLoginRoute(IIntentTemplate)"/>
    public static string GetLoginRoute(this IMetadataManager metadataManager, string applicationId)
    {
        var loginPage = metadataManager.UserInterface(applicationId)
            .GetComponentModels()
            .FirstOrDefault(component =>
                component.InternalElement.Metadata.TryGetValue(AuthPageIdMetadataKey, out var pageId) &&
                LoginPageIds.Contains(pageId));

        if (loginPage is null || !loginPage.HasPage())
        {
            return DefaultLoginRoute;
        }

        var route = loginPage.GetPage().Route();

        if (string.IsNullOrWhiteSpace(route))
        {
            return DefaultLoginRoute;
        }

        return route.StartsWith('/') ? route : $"/{route}";
    }
}
