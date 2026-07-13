using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.Repositories.DapperHybrid
{
    public class NugetPackages : INugetPackages
    {
        public const string DapperPackageName = "Dapper";

        public void RegisterPackages()
        {
            NugetRegistry.Register(DapperPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("2.1.79"),
                        ( >= 8, >= 0) => new PackageVersion("2.1.79"),
                        ( >= 2, >= 0) => new PackageVersion("2.1.79")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "10.0.8")
                            .WithNugetDependency("System.Reflection.Emit.Lightweight", "4.7.0")
                            .WithNugetDependency("System.Threading.Tasks.Extensions", "4.6.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{DapperPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo Dapper(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DapperPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
