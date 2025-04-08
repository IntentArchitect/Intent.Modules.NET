using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage
{
    public class NugetPackages : INugetPackages
    {
        public const string AzureDataTablesPackageName = "Azure.Data.Tables";

        public void RegisterPackages()
        {
            NugetRegistry.Register(AzureDataTablesPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("12.10.0")
                            .WithNugetDependency("Azure.Core", "1.44.1"),
                        ( >= 2, 0) => new PackageVersion("12.10.0")
                            .WithNugetDependency("Azure.Core", "1.44.1")
                            .WithNugetDependency("System.Text.Json", "6.0.10"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureDataTablesPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureDataTables(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureDataTablesPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
