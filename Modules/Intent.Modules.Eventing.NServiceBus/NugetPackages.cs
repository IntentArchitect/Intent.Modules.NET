using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.Eventing.NServiceBus
{
    public class NugetPackages : INugetPackages
    {
        public const string NServiceBusPackageName = "NServiceBus";
        public const string NServiceBusRabbitMQPackageName = "NServiceBus.RabbitMQ";

        public void RegisterPackages()
        {
            NugetRegistry.Register(NServiceBusPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("8.2.6"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusPackageName}'"),
                    }
                );
            NugetRegistry.Register(NServiceBusRabbitMQPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 8, >= 0) => new PackageVersion("9.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{NServiceBusRabbitMQPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo NServiceBus(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo NServiceBusRabbitMQ(IOutputTarget outputTarget) => NugetRegistry.GetVersion(NServiceBusRabbitMQPackageName, outputTarget.GetMaxNetAppVersion());
    }
}