using Intent.Blazor.Authentication.Api;
using Intent.Engine;
using Intent.Modelers.UI.Api;
using Intent.Modules.Blazor.Templates.Templates.Client;
using Intent.Modules.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Blazor.Authentication.Api.UserInterfacePackageModelStereotypeExtensions.Security;

namespace Intent.Modules.Blazor.Authentication.Api;

public static class SecurityHelperExtensions
{
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
}
