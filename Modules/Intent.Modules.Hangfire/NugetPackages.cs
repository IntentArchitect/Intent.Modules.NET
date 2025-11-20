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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.8.22")
                            .WithNugetDependency("Hangfire.NetCore", "1.8.22")
                            .WithNugetDependency("Microsoft.AspNetCore.Antiforgery", "2.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.8.22")
                            .WithNugetDependency("Newtonsoft.Json", "11.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireInMemoryPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.0.0")
                            .WithNugetDependency("Hangfire.Core", "1.8.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireSqlServerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.8.22")
                            .WithNugetDependency("Hangfire.Core", "1.8.22"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("6.1.3")
                            .WithNugetDependency("Azure.Core", "1.47.1")
                            .WithNugetDependency("Azure.Identity", "1.14.2")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "9.0.4")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.4")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "7.7.1")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "7.7.1")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "9.0.4")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "9.0.4")
                            .WithNugetDependency("System.Text.Json", "9.0.5"),
                        ( >= 8, >= 0) => new PackageVersion("6.1.3")
                            .WithNugetDependency("Azure.Core", "1.47.1")
                            .WithNugetDependency("Azure.Identity", "1.14.2")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "7.7.1")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "7.7.1")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1")
                            .WithNugetDependency("System.Text.Json", "8.0.5"),
                        ( >= 2, >= 0) => new PackageVersion("6.1.3")
                            .WithNugetDependency("Azure.Core", "1.47.1")
                            .WithNugetDependency("Azure.Identity", "1.14.2")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "9.0.4")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.4")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "7.7.1")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "7.7.1")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "9.0.4")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "9.0.4")
                            .WithNugetDependency("System.Text.Json", "9.0.5"),
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