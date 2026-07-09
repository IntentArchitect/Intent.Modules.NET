using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Dapper
{
    public class NugetPackages : INugetPackages
    {
        public const string DapperPackageName = "Dapper";
        public const string SystemDataSqlClientPackageName = "System.Data.SqlClient";

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
            NugetRegistry.Register(SystemDataSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("4.9.1")
                            .WithNugetDependency("runtime.native.System.Data.SqlClient.sni", "4.4.0"),
                        ( >= 6, >= 0) => new PackageVersion("4.9.1")
                            .WithNugetDependency("runtime.native.System.Data.SqlClient.sni", "4.4.0"),
                        ( >= 2, >= 0) => new PackageVersion("4.9.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SystemDataSqlClientPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo Dapper(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DapperPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo SystemDataSqlClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SystemDataSqlClientPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
