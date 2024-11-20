using System;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Nuget;
using Intent.Modules.Common.VisualStudio;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.NugetPackages", Version = "1.0")]

namespace Intent.Modules.HotChocolate.GraphQL
{
    public class NugetPackages : INugetPackages
    {
        public const string HotChocolatePackageName = "HotChocolate";

        public void RegisterPackages()
        {
            NugetRegistry.Register(HotChocolatePackageName,
                (framework) => framework switch
                    {
                        ( >= 8, 0) => new PackageVersion("14.1.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.1.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.1.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types", "14.1.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.1.0")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "14.1.0")
                            .WithNugetDependency("HotChocolate.Pagination.Mappings", "14.1.0"),
                        ( >= 7, 0) => new PackageVersion("14.1.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.1.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.1.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types", "14.1.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.1.0")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "14.1.0")
                            .WithNugetDependency("HotChocolate.Pagination.Mappings", "14.1.0"),
                        ( >= 6, 0) => new PackageVersion("14.1.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.1.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.1.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types", "14.1.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.1.0"),
                        ( >= 2, 0) => new PackageVersion("14.1.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.1.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.1.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.1.0")
                            .WithNugetDependency("HotChocolate.Types", "14.1.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.1.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolatePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolate(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolatePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
