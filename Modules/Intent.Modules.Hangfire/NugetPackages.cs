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
                        ( >= 2, >= 0) => new PackageVersion("1.8.25")
                            .WithNugetDependency("Hangfire.NetCore", "1.8.25")
                            .WithNugetDependency("Microsoft.AspNetCore.Antiforgery", "2.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("1.8.25")
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
                        ( >= 2, >= 0) => new PackageVersion("1.8.25")
                            .WithNugetDependency("Hangfire.Core", "1.8.25"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("7.1.1")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "9.0.18")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "7.1.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "7.1.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "7.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.18")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "9.0.18")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "9.0.18")
                            .WithNugetDependency("System.Threading.RateLimiting", "9.0.18"),
                        ( >= 8, >= 0) => new PackageVersion("7.1.1")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "7.1.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "7.1.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "7.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1")
                            .WithNugetDependency("System.Threading.RateLimiting", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("7.1.1")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Bcl.TimeProvider", "8.0.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "7.1.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "7.1.1")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "7.1.0")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1")
                            .WithNugetDependency("System.Text.Json", "10.0.3")
                            .WithNugetDependency("System.Threading.Channels", "10.0.3")
                            .WithNugetDependency("System.Threading.RateLimiting", "8.0.0"),
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