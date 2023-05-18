using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates
{
    public static class NuGetPackages
    {
        public static INugetPackageInfo HotChocolateAspNetCore => new NugetPackageInfo("HotChocolate.AspNetCore", "13.0.5");
        public static INugetPackageInfo HotChocolateAspNetCoreAuthorization => new NugetPackageInfo("HotChocolate.AspNetCore.Authorization", "13.0.5");
    }
}