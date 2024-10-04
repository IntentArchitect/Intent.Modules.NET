using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Hangfire
{
    public class NugetPackages : INugetPackages
    {
        public const string HangfireAspNetCorePackageName = "Hangfire.AspNetCore";
        public const string HangfireCorePackageName = "Hangfire.Core";
        public const string HangfireInMemoryPackageName = "Hangfire.InMemory";
        public const string HangfireSqlServerPackageName = "Hangfire.SqlServer";
        public const string MicrosoftDataSqlClientPackageName = "Microsoft.Data.SqlClient";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HangfireAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.8.14"),
                        ( >= 2, 0) => new PackageVersion("1.8.14"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 6, 0) => new PackageVersion("1.8.14"),
                        ( >= 2, 0) => new PackageVersion("1.8.14"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireInMemoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("0.10.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireSqlServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("1.8.14"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("5.2.2"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftDataSqlClientPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HangfireAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HangfireAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo HangfireCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HangfireCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo HangfireInMemory(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HangfireInMemoryPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo HangfireSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HangfireSqlServerPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftDataSqlClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftDataSqlClientPackageName, outputTarget.GetMaxNetAppVersion());
    }
}