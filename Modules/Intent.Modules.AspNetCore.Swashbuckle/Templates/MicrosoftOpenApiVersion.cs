using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using NuGet.Versioning;

namespace Intent.Modules.AspNetCore.Swashbuckle.Templates;

public static class MicrosoftOpenApiVersion
{
    // Swashbuckle 11 depends on Microsoft.OpenApi 3.x, where request body content is an IOpenApiMediaType with a
    // read-only Schema. Decided from the Swashbuckle version the application gets, so the generated code moves
    // together with the NuGet entry. Below .NET 8 the Microsoft.OpenApi 1.x namespaces apply.
    public static bool IsV3(IOutputTarget outputTarget)
    {
        return outputTarget.GetMaxNetAppVersion().Major >= 8 &&
            NuGetVersion.Parse(NugetPackages.SwashbuckleAspNetCore(outputTarget).Version).Major >= 11;
    }
}
