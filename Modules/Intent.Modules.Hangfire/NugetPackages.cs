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
                        ( >= 2, 0) => new PackageVersion("1.8.15")
                            .WithNugetDependency("Hangfire.NetCore", "1.8.15")
                            .WithNugetDependency("Microsoft.AspNetCore.Antiforgery", "2.0.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Abstractions", "2.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.8.15")
                            .WithNugetDependency("Newtonsoft.Json", "11.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireInMemoryPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.0.0")
                            .WithNugetDependency("Hangfire.Core", "1.8.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireInMemoryPackageName}'"),
                    }
                );
            NugetRegistry.Register(HangfireSqlServerPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 0) => new PackageVersion("1.8.15")
                            .WithNugetDependency("Hangfire.Core", "1.8.15"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HangfireSqlServerPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("5.2.2")
                            .WithNugetDependency("Azure.Identity", "1.11.4")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "5.2.0")
                            .WithNugetDependency("Microsoft.Identity.Client", "4.61.3")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "6.35.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "6.35.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.0")
                            .WithNugetDependency("System.Runtime.Caching", "8.0.0"),
                        ( >= 6, 0) => new PackageVersion("5.2.2")
                            .WithNugetDependency("Azure.Identity", "1.11.4")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "5.2.0")
                            .WithNugetDependency("Microsoft.Identity.Client", "4.61.3")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "6.35.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "6.35.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "6.0.1")
                            .WithNugetDependency("System.Runtime.Caching", "6.0.0"),
                        ( >= 2, 1) => new PackageVersion("5.2.2")
                            .WithNugetDependency("Azure.Identity", "1.11.4")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "5.2.0")
                            .WithNugetDependency("Microsoft.Identity.Client", "4.61.3")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "6.35.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "6.35.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("Microsoft.Win32.Registry", "5.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "6.0.1")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "6.0.1")
                            .WithNugetDependency("System.Runtime.Caching", "6.0.0")
                            .WithNugetDependency("System.Runtime.Loader", "4.3.0")
                            .WithNugetDependency("System.Security.Cryptography.Cng", "5.0.0")
                            .WithNugetDependency("System.Security.Principal.Windows", "5.0.0")
                            .WithNugetDependency("System.Text.Encoding.CodePages", "6.0.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "6.0.0"),
                        ( >= 2, 0) => new PackageVersion("5.2.2")
                            .WithNugetDependency("Azure.Identity", "1.11.4")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "5.2.0")
                            .WithNugetDependency("Microsoft.Identity.Client", "4.61.3")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "6.35.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "6.35.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("Microsoft.Win32.Registry", "5.0.0")
                            .WithNugetDependency("System.Buffers", "4.5.1")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "6.0.1")
                            .WithNugetDependency("System.Diagnostics.DiagnosticSource", "6.0.1")
                            .WithNugetDependency("System.Runtime.Caching", "6.0.0")
                            .WithNugetDependency("System.Runtime.Loader", "4.3.0")
                            .WithNugetDependency("System.Security.Cryptography.Cng", "5.0.0")
                            .WithNugetDependency("System.Security.Principal.Windows", "5.0.0")
                            .WithNugetDependency("System.Text.Encoding.CodePages", "6.0.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "6.0.0"),
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