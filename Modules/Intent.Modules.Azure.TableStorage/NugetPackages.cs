using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Azure.TableStorage
{
    public class NugetPackages
    {
        public const string AzureDataTablesPackageName = "Azure.Data.Tables";

        static NugetPackages()
        {
            NugetRegistry.Register(AzureDataTablesPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("12.9.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{AzureDataTablesPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo AzureDataTables(IOutputTarget outputTarget) => NugetRegistry.GetVersion(AzureDataTablesPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
