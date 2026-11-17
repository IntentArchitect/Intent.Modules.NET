using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using NuGet.Versioning;

namespace Intent.Modules.AspNetCore.Scalar.Templates;

public static class MicrosoftOpenApiVersion
{
    // Microsoft.AspNetCore.OpenApi 11 depends on Microsoft.OpenApi 3.x, where request and response content is an
    // IOpenApiMediaType with a read-only Schema. Decided from the package version the application gets, so the
    // generated code moves together with the NuGet entry. Below .NET 10 there is no Microsoft.OpenApi 2.x path to refine.
    public static bool IsV3(IOutputTarget outputTarget)
    {
        return outputTarget.GetMaxNetAppVersion().Major >= 10 &&
            NuGetVersion.Parse(NugetPackages.MicrosoftAspNetCoreOpenApi(outputTarget).Version).Major >= 11;
    }
}
