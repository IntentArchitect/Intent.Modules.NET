using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Application.Mediator.DomainEvents
{
    public class NugetPackages : INugetPackages
    {
        public const string MediatorAbstractionsPackageName = "Mediator.Abstractions";
        public const string MicrosoftExtensionsLoggingPackageName = "Microsoft.Extensions.Logging";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MediatorAbstractionsPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("3.0.2", locked: true),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MediatorAbstractionsPackageName}'"),
                    }
                );
            NugetRegistry.Register(MicrosoftExtensionsLoggingPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("10.0.9"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MicrosoftExtensionsLoggingPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MediatorAbstractions(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MediatorAbstractionsPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo MicrosoftExtensionsLogging(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MicrosoftExtensionsLoggingPackageName, outputTarget.GetMaxNetAppVersion());
    }
}