using System;
using System.Linq;
using Microsoft.Build.Locator;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    internal static class MsBuildLoaderHelper
    {
        private const int MinimumSdkMajorVersion = 6;
        private static readonly Lazy<bool> IsLoaded = new(() =>
        {
            if (MSBuildLocator.IsRegistered)
            {
                return true;
            }

            var instance = MSBuildLocator.QueryVisualStudioInstances()
                .FirstOrDefault(x => x.Version.Major >= MinimumSdkMajorVersion);
            if (instance == null)
            {
                return false;
            }

            MSBuildLocator.RegisterInstance(instance);
            return true;
        });

        /// <summary>
        /// For the RoslynWeaver module to be able to successfully load .sln workspaces, it has to use the full
        /// .NET SDK and <see cref="MSBuildLocator"/> allows you to find it on a computer and load related
        /// MSBuild assemblies from it. Because .NET Framework .csproj file generation in this module requires
        /// MSBuild assemblies, this is called in their Registration classes and for the
        /// <c>&lt;PackageReference Include="Microsoft.Build" /"&gt;</c> element in this project's .csproj
        /// has <c>&lt;ExcludeAssets&gt;runtime&lt;/ExcludeAssets&gt;</c> added to it.
        /// </summary>
        public static void EnsureMsBuildLoaded()
        {
            if (IsLoaded.Value)
            {
                return;
            }

            throw new($"No .NET SDK with version >= {MinimumSdkMajorVersion} installed. " +
                      "The .NET SDK can be downloaded from https://dotnet.microsoft.com/download.");

        }
    }
}
