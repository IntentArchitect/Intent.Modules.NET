using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Grpc
{
    public class NugetPackages : INugetPackages
    {
        public const string GrpcAspNetCorePackageName = "Grpc.AspNetCore";
        public const string GrpcAspNetCoreServerReflectionPackageName = "Grpc.AspNetCore.Server.Reflection";
        public const string GrpcStatusProtoPackageName = "Grpc.StatusProto";

        public void RegisterPackages()
        {
            NugetRegistry.Register(GrpcAspNetCorePackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server.ClientFactory", "2.70.0")
                            .WithNugetDependency("Grpc.Tools", "2.70.0"),
                        ( >= 8, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server.ClientFactory", "2.70.0")
                            .WithNugetDependency("Grpc.Tools", "2.70.0"),
                        ( >= 7, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server.ClientFactory", "2.70.0")
                            .WithNugetDependency("Grpc.Tools", "2.70.0"),
                        ( >= 6, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server.ClientFactory", "2.70.0")
                            .WithNugetDependency("Grpc.Tools", "2.70.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{GrpcAspNetCorePackageName}'"),
                    }
                );
            NugetRegistry.Register(GrpcAspNetCoreServerReflectionPackageName,
                (framework) => framework switch
                    {
                        ( >= 9, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server", "2.70.0")
                            .WithNugetDependency("Grpc.Reflection", "2.70.0"),
                        ( >= 8, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server", "2.70.0")
                            .WithNugetDependency("Grpc.Reflection", "2.70.0"),
                        ( >= 7, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server", "2.70.0")
                            .WithNugetDependency("Grpc.Reflection", "2.70.0"),
                        ( >= 6, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Protobuf", "3.27.0")
                            .WithNugetDependency("Grpc.AspNetCore.Server", "2.70.0")
                            .WithNugetDependency("Grpc.Reflection", "2.70.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{GrpcAspNetCoreServerReflectionPackageName}'"),
                    }
                );
            NugetRegistry.Register(GrpcStatusProtoPackageName,
                (framework) => framework switch
                    {
                        ( >= 2, 1) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.15.0")
                            .WithNugetDependency("Grpc.Core.Api", "2.70.0"),
                        ( >= 2, 0) => new PackageVersion("2.70.0")
                            .WithNugetDependency("Google.Api.CommonProtos", "2.15.0")
                            .WithNugetDependency("Grpc.Core.Api", "2.70.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{GrpcStatusProtoPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo GrpcAspNetCore(IOutputTarget outputTarget) => NugetRegistry.GetVersion(GrpcAspNetCorePackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo GrpcAspNetCoreServerReflection(IOutputTarget outputTarget) => NugetRegistry.GetVersion(GrpcAspNetCoreServerReflectionPackageName, outputTarget.GetMaxNetAppVersion());

        public static NugetPackageInfo GrpcStatusProto(IOutputTarget outputTarget) => NugetRegistry.GetVersion(GrpcStatusProtoPackageName, outputTarget.GetMaxNetAppVersion());
    }
}