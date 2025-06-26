using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Application.Contracts.Clients.Templates;

public static class ExtensionMethods
{
    public const string HttpSettingsDefinitionId = "b4581ed2-42ec-4ae2-83dd-dcdd5f0837b6";

    public static bool HasHttpSettings(this IHasStereotypes? hasStereotypes)
    {
        return hasStereotypes?.HasStereotype(HttpSettingsDefinitionId) == true;
    }
}