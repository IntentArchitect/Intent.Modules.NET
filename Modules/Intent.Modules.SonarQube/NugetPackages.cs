using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.SonarQube
{
    public class NugetPackages : INugetPackages
    {
        public const string SonarAnalyzerCSharpPackageName = "SonarAnalyzer.CSharp";

        public void RegisterPackages()
        {
            NugetRegistry.Register(SonarAnalyzerCSharpPackageName,
                (framework) => framework switch
                    {
                        ( >= 0, 0) => new PackageVersion("9.32.0.97167")
                            .SpecifyAssetsBehaviour(privateAssets: new[] { "all" }, includeAssets: new[] { "runtime", "build", "native", "contentFiles", "analyzers", "buildTransitive" }),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{SonarAnalyzerCSharpPackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo SonarAnalyzerCSharp(IOutputTarget outputTarget) => NugetRegistry.GetVersion(SonarAnalyzerCSharpPackageName, outputTarget.GetMaxNetAppVersion());
    }
}