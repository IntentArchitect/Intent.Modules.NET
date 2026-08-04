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
        public const string MicrosoftDataSqlClientPackageName = "Microsoft.Data.SqlClient";

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
            NugetRegistry.Register(MicrosoftDataSqlClientPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("7.0.2")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "9.0.13")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "7.0.2")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "7.0.2")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.13")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "9.0.13")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "9.0.13"),
                        ( >= 8, >= 0) => new PackageVersion("7.0.2")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "7.0.2")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "7.0.2")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1"),
                        ( >= 2, >= 0) => new PackageVersion("7.0.2")
                            .WithNugetDependency("Microsoft.Bcl.Cryptography", "8.0.0")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Extensions.Abstractions", "7.0.2")
                            .WithNugetDependency("Microsoft.Data.SqlClient.Internal.Logging", "7.0.2")
                            .WithNugetDependency("Microsoft.Data.SqlClient.SNI.runtime", "6.0.2")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "8.0.1")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.16.0")
                            .WithNugetDependency("Microsoft.IdentityModel.Protocols.OpenIdConnect", "8.16.0")
                            .WithNugetDependency("Microsoft.SqlServer.Server", "1.0.0")
                            .WithNugetDependency("System.Configuration.ConfigurationManager", "8.0.1")
                            .WithNugetDependency("System.Security.Cryptography.Pkcs", "8.0.1")
                            .WithNugetDependency("System.Text.Json", "10.0.3")
                            .WithNugetDependency("System.Threading.Channels", "10.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftDataSqlClientPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo Dapper(IOutputTarget outputTarget) => NugetRegistry.GetVersion(DapperPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftDataSqlClient(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftDataSqlClientPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
