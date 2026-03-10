using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.AspNetCore.JsonPatch
{
    public class NugetPackages : INugetPackages
    {
        public const string MorcatkoAspNetCoreJsonMergePatchNewtonsoftJsonPackageName = "Morcatko.AspNetCore.JsonMergePatch.NewtonsoftJson";

        public void RegisterPackages()
        {
            NugetRegistry.Register(MorcatkoAspNetCoreJsonMergePatchNewtonsoftJsonPackageName,
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 6, >= 0) => new PackageVersion("6.0.4")
                            .WithNugetDependency("Microsoft.AspNetCore.Mvc.NewtonsoftJson", "6.0.0")
                            .WithNugetDependency("Morcatko.AspNetCore.JsonMergePatch.Document", "6.0.4"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{MorcatkoAspNetCoreJsonMergePatchNewtonsoftJsonPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo MorcatkoAspNetCoreJsonMergePatchNewtonsoftJson(IOutputTarget outputTarget) => NugetRegistry.GetVersion(MorcatkoAspNetCoreJsonMergePatchNewtonsoftJsonPackageName, outputTarget.GetMaxNetAppVersion());
    }
}