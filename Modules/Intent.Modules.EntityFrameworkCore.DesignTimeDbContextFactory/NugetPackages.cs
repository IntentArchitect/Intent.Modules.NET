using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DesignTimeDbContextFactory
{
    public class NugetPackages : INugetPackages
    {
        public const string MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName = "Microsoft.Extensions.Configuration.EnvironmentVariables";
        public const string MicrosoftExtensionsConfigurationFileExtensionsPackageName = "Microsoft.Extensions.Configuration.FileExtensions";
        public const string MicrosoftExtensionsConfigurationJsonPackageName = "Microsoft.Extensions.Configuration.Json";
        public const string MicrosoftExtensionsConfigurationUserSecretsPackageName = "Microsoft.Extensions.Configuration.UserSecrets";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0"),
                        ( >= 8, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0"),
                        ( >= 2, 0) => new PackageVersion("9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationFileExtensionsPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.3"),
                        ( >= 8, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.3"),
                        ( >= 2, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Primitives", "9.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationFileExtensionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationJsonPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3"),
                        ( >= 8, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("System.Text.Json", "9.0.3"),
                        ( >= 2, 1) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("System.Text.Json", "9.0.3"),
                        ( >= 2, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.FileExtensions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("System.Text.Json", "9.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationJsonPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsConfigurationUserSecretsPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.3"),
                        ( >= 8, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.3"),
                        ( >= 2, 0) => new PackageVersion("9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.Configuration.Json", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Abstractions", "9.0.3")
                            .WithNugetDependency("Microsoft.Extensions.FileProviders.Physical", "9.0.3"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsConfigurationUserSecretsPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MicrosoftExtensionsConfigurationFileExtensions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationFileExtensionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationJsonPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationUserSecrets(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationUserSecretsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsConfigurationEnvironmentVariables(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsConfigurationEnvironmentVariablesPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
