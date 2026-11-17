using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;

namespace Intent.Modules.AspNetCore.Scalar.Templates;

public static class HttpQueryHelper
{
    // Microsoft.OpenApi 2.x and HttpMethod.Query only line up from .NET 10, and older targets have no QUERY-aware pipeline to describe.
    // On Microsoft.OpenApi 3.x (Microsoft.AspNetCore.OpenApi 11) the framework writes OpenAPI 3.2 and describes QUERY itself.
    public static bool NeedsHttpQueryOpenApi(IOutputTarget outputTarget)
    {
        if (outputTarget.GetMaxNetAppVersion().Major < 10 || MicrosoftOpenApiVersion.IsV3(outputTarget))
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
