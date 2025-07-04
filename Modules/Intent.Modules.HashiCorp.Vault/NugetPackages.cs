using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HashiCorp.Vault
{
    public class NugetPackages : INugetPackages
    {
        public const string VaultSharpPackageName = "VaultSharp";

        public void RegisterPackages()
        {
            NugetRegistry.Register(VaultSharpPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("1.17.5.1"),
                        ( >= 7, >= 0) => new PackageVersion("1.17.5.1"),
                        ( >= 6, >= 0) => new PackageVersion("1.17.5.1")
                            .WithNugetDependency("System.Text.Json", "8.0.4"),
                        ( >= 2, >= 1) => new PackageVersion("1.17.5.1")
                            .WithNugetDependency("System.Text.Json", "8.0.4"),
                        ( >= 2, >= 0) => new PackageVersion("1.17.5.1")
                            .WithNugetDependency("System.Text.Json", "8.0.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{VaultSharpPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo VaultSharp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(VaultSharpPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
