using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL.AspNetCore
{
    public class NugetPackages : INugetPackages
    {
        public const string HotChocolateAspNetCorePackageName = "HotChocolate.AspNetCore";
        public const string HotChocolateAspNetCoreAuthorizationPackageName = "HotChocolate.AspNetCore.Authorization";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HotChocolateAspNetCorePackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("15.1.3")
                            .WithNugetDependency("HotChocolate.Transport.Sockets", "15.1.3")
                            .WithNugetDependency("HotChocolate", "15.1.3")
                            .WithNugetDependency("HotChocolate.Subscriptions.InMemory", "15.1.3")
                            .WithNugetDependency("HotChocolate.Types.Scalars.Upload", "15.1.3")
                            .WithNugetDependency("HotChocolate.Utilities.DependencyInjection", "15.1.3")
                            .WithNugetDependency("ChilliCream.Nitro.App", "26.0.4"),
                        ( >= 8, >= 0) => new PackageVersion("15.1.3")
                            .WithNugetDependency("HotChocolate.Transport.Sockets", "15.1.3")
                            .WithNugetDependency("HotChocolate", "15.1.3")
                            .WithNugetDependency("HotChocolate.Subscriptions.InMemory", "15.1.3")
                            .WithNugetDependency("HotChocolate.Types.Scalars.Upload", "15.1.3")
                            .WithNugetDependency("HotChocolate.Utilities.DependencyInjection", "15.1.3")
                            .WithNugetDependency("ChilliCream.Nitro.App", "26.0.4"),
                        ( >= 7, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate.Transport.Sockets", "14.3.0")
                            .WithNugetDependency("HotChocolate", "14.3.0")
                            .WithNugetDependency("HotChocolate.Subscriptions.InMemory", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Scalars.Upload", "14.3.0")
                            .WithNugetDependency("HotChocolate.Utilities.DependencyInjection", "14.3.0")
                            .WithNugetDependency("ChilliCream.Nitro.App", "20.0.2"),
                        ( >= 6, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate.Transport.Sockets", "14.3.0")
                            .WithNugetDependency("HotChocolate", "14.3.0")
                            .WithNugetDependency("HotChocolate.Subscriptions.InMemory", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Scalars.Upload", "14.3.0")
                            .WithNugetDependency("HotChocolate.Utilities.DependencyInjection", "14.3.0")
                            .WithNugetDependency("ChilliCream.Nitro.App", "20.0.2"),
                        ( >= 2, >= 0) => new PackageVersion("10.5.5")
                            .WithNugetDependency("HotChocolate", "10.5.5")
                            .WithNugetDependency("HotChocolate.AspNetCore.Abstractions", "10.5.5")
                            .WithNugetDependency("HotChocolate.AspNetCore.Authorization", "10.5.5")
                            .WithNugetDependency("HotChocolate.AspNetCore.HttpGetSchema", "10.5.5")
                            .WithNugetDependency("HotChocolate.AspNetCore.HttpGet", "10.5.5")
                            .WithNugetDependency("HotChocolate.AspNetCore.HttpPost", "10.5.5")
                            .WithNugetDependency("HotChocolate.AspNetCore.Subscriptions", "10.5.5")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection.Abstractions", "2.1.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolateAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(HotChocolateAspNetCoreAuthorizationPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 9, >= 0) => new PackageVersion("15.1.3")
                            .WithNugetDependency("HotChocolate", "15.1.3"),
                        ( >= 8, >= 0) => new PackageVersion("15.1.3")
                            .WithNugetDependency("HotChocolate", "15.1.3"),
                        ( >= 7, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate", "14.3.0"),
                        ( >= 6, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate", "14.3.0"),
                        ( >= 2, >= 0) => new PackageVersion("13.0.5")
                            .WithNugetDependency("HotChocolate", "13.0.5")
                            .WithNugetDependency("Microsoft.AspNetCore.Authorization", "3.1.4")
                            .WithNugetDependency("Microsoft.Extensions.DependencyInjection", "3.1.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolateAspNetCoreAuthorizationPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolateAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolateAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo HotChocolateAspNetCoreAuthorization(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolateAspNetCoreAuthorizationPackageName, outputTarget.GetMaxNetAppVersion());
    }
}
