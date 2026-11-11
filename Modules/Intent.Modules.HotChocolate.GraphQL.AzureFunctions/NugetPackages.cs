using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AzureFunctions
{
    public class NugetPackages : INugetPackages
    {
        public const string HotChocolateAzureFunctionsPackageName = "HotChocolate.AzureFunctions";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HotChocolateAzureFunctionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("16.7.5")
                            .WithNugetDependency("ChilliCream.Nitro.App", "32.0.2")
                            .WithNugetDependency("HotChocolate.AspNetCore", "16.7.5")
                            .WithNugetDependency("Microsoft.Azure.Functions.Extensions", "1.1.0")
                            .WithNugetDependency("Microsoft.Azure.WebJobs", "3.0.41"),
                        ( >= 9, >= 0) => new PackageVersion("16.7.5")
                            .WithNugetDependency("ChilliCream.Nitro.App", "32.0.2")
                            .WithNugetDependency("HotChocolate.AspNetCore", "16.7.5")
                            .WithNugetDependency("Microsoft.Azure.Functions.Extensions", "1.1.0")
                            .WithNugetDependency("Microsoft.Azure.WebJobs", "3.0.41"),
                        ( >= 8, >= 0) => new PackageVersion("16.7.5")
                            .WithNugetDependency("ChilliCream.Nitro.App", "32.0.2")
                            .WithNugetDependency("HotChocolate.AspNetCore", "16.7.5")
                            .WithNugetDependency("Microsoft.Azure.Functions.Extensions", "1.1.0")
                            .WithNugetDependency("Microsoft.Azure.WebJobs", "3.0.41"),
                        ( >= 7, >= 0) => new PackageVersion("14.3.5")
                            .WithNugetDependency("ChilliCream.Nitro.App", "23.0.4")
                            .WithNugetDependency("HotChocolate.AspNetCore", "14.3.5")
                            .WithNugetDependency("Microsoft.Azure.Functions.Extensions", "1.1.0"),
                        ( >= 6, >= 0) => new PackageVersion("14.3.5")
                            .WithNugetDependency("ChilliCream.Nitro.App", "23.0.4")
                            .WithNugetDependency("HotChocolate.AspNetCore", "14.3.5")
                            .WithNugetDependency("Microsoft.Azure.Functions.Extensions", "1.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolateAzureFunctionsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolateAzureFunctions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolateAzureFunctionsPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
