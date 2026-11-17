using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates;

public static class HttpQueryHelper
{
    // Swashbuckle 10 and HttpMethod.Query only line up from .NET 10, and older targets have no QUERY-aware pipeline to describe.
    public static bool NeedsHttpQueryOpenApi(IOutputTarget outputTarget)
    {
        if (outputTarget.GetMaxNetAppVersion().Major < 10)
        {
            return false;
        }

        var executionContext = outputTarget.ExecutionContext;
        var servicesDesigner = executionContext.MetadataManager.GetDesigner(executionContext.GetApplicationConfig().Id, Designers.Services);

        return servicesDesigner.Elements.Any(IsQueryEndpoint);
    }

    private static bool IsQueryEndpoint(IElement element)
    {
        return element.GetStereotype("Http Settings")?.TryGetProperty("Verb", out var verb) == true && verb.Value == "QUERY";
    }
}
