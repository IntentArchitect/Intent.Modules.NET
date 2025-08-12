using System;
using Intent.Engine;
using Intent.Modules.Application.DependencyInjection.MediatR.Settings;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.DependencyInjection.MediatR
{
    public class NugetPackages : INugetPackages
    {
        [IntentIgnore]
        private readonly IApplicationSettingsProvider _applicationSettingsProvider;
        
        public const string MediatRPackageName = "MediatR";

        [IntentIgnore]
        public NugetPackages(IApplicationSettingsProvider applicationSettingsProvider)
        {
            _applicationSettingsProvider = applicationSettingsProvider;
        }
        
        public void RegisterPackages()
        {
            //IntentIgnore
            if (_applicationSettingsProvider.GetMediatRSettings().UsePreCommercialVersion())
            {
                NugetRegistry.Register(MediatRPackageName,
                    (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("12.5.0", locked: true)
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
                        ( >= 2, >= 0) => new PackageVersion("12.5.0", locked: true)
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
            }
            
            NugetRegistry.Register(MediatRPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("13.0.0")
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.0.1"),
                        ( >= 8, >= 0) => new PackageVersion("13.0.0")
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.0.1"),
                        ( >= 2, >= 0) => new PackageVersion("13.0.0")
                            .WithNugetDependency("MediatR.Contracts", "2.0.1")
                            .WithNugetDependency("Microsoft.Bcl.AsyncInterfaces", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "8.0.0")
                            .WithNugetDependency("Microsoft.Extensions.Logging", "8.0.0")
                            .WithNugetDependency("Microsoft.IdentityModel.JsonWebTokens", "8.0.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatRPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MediatR(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatRPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
