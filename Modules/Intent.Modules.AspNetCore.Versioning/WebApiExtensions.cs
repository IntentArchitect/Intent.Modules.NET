using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Metadata.WebApi.Api;

namespace Intent.Modules.AspNetCore.Versioning;

public static class WebApiExtensions
{
    public static ServiceModelStereotypeExtensions.ApiVersion GetApiVersion(this IHasStereotypes model)
    {
        var stereotype = model.GetStereotype("Api Version");
        return stereotype != null ? new ServiceModelStereotypeExtensions.ApiVersion(stereotype) : null;
    }
    
    public static bool HasApiVersion(this IHasStereotypes model)
    {
        return model.HasStereotype("Api Version");
    }
}