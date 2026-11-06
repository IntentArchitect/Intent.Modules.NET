using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Metadata.WebApi.Api;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;

namespace Intent.Modules.AspNetCore.Controllers.Templates;

public static class HttpQueryHelper
{
    public static bool NeedsHttpQueryInfrastructure(IMetadataManager metadataManager, string applicationId)
    {
        return metadataManager.Services(applicationId).Elements
            .Select(x => x.GetStereotype(OperationModelStereotypeExtensions.HttpSettings.DefinitionId))
            .Any(x => x != null && new OperationModelStereotypeExtensions.HttpSettings(x).Verb().IsQUERY());
    }
}
