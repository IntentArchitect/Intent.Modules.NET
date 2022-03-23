using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore.Templates
{
    public static class NuGetPackages
    {
        public static INugetPackageInfo HotChocolateAspNetCore => new NugetPackageInfo("HotChocolate.AspNetCore", "12.7.0");
    }
}