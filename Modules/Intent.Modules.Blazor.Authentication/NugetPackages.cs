using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Blazor.Authentication
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftAspNetCoreHttpAbstractionsPackageName = "Microsoft.AspNetCore.Http.Abstractions";
        public const string MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName = "Microsoft.AspNetCore.Identity.EntityFrameworkCore";
        public const string MicrosoftEntityFrameworkCoreSqlServerPackageName = "Microsoft.EntityFrameworkCore.SqlServer";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftAspNetCoreHttpAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Http.Features", "2.3.0")
                            .WithNugetDependency("System.Text.Encodings.Web", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreHttpAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "9.0.10"),
                        ( >= 8, >= 0) => new PackageVersion("8.0.21")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "8.0.21")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "8.0.21"),
                        ( >= 7, >= 0) => new PackageVersion("7.0.20")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.20")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "7.0.20"),
                        ( >= 6, >= 0) => new PackageVersion("6.0.36")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "6.0.36")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "6.0.36"),
                        ( >= 2, >= 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "5.0.17"),
                        ( >= 2, >= 0) => new PackageVersion("2.3.0")
                            .WithNugetDependency("Microsoft.AspNetCore.Identity", "2.3.0")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "2.1.14")
                            .WithNugetDependency("Microsoft.Extensions.Identity.Stores", "2.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftEntityFrameworkCoreSqlServerPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("9.0.10")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "5.1.6")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Caching.Memory", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.10")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "9.0.10")
                            .WithNugetDependency("System.Formats.Asn1", "9.0.10")
                            .WithNugetDependency("System.Text.Json", "9.0.10"),
                        ( >= 6, >= 0) => new PackageVersion("7.0.20")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "5.1.5")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "7.0.20"),
                        ( >= 2, >= 1) => new PackageVersion("5.0.17")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "2.0.1")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "5.0.17"),
                        ( >= 2, >= 0) => new PackageVersion("3.1.32")
                            .WithNugetDependency("Microsoft.Data.SqlClient", "1.1.3")
                            .WithNugetDependency("Microsoft.EntityFrameworkCore.Relational", "3.1.32"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftEntityFrameworkCoreSqlServerPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftAspNetCoreHttpAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreHttpAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftAspNetCoreIdentityEntityFrameworkCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftAspNetCoreIdentityEntityFrameworkCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftEntityFrameworkCoreSqlServer(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftEntityFrameworkCoreSqlServerPackageName, outputTarget.GetMaxNetAppVersion());
    }
}