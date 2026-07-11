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
                (framework) => (framework.Major, framework.Minor) switch
                    {
                        ( >= 10, >= 0) => new PackageVersion("16.4.0")
                            .WithNugetDependency("HotChocolate.Authorization", "16.4.0")
                            .WithNugetDependency("HotChocolate.Execution.Projections", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination.Extensions", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types", "16.4.0")
                            .WithNugetDependency("HotChocolate.Validation", "16.4.0")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "16.4.0"),
                        ( >= 9, >= 0) => new PackageVersion("16.4.0")
                            .WithNugetDependency("HotChocolate.Authorization", "16.4.0")
                            .WithNugetDependency("HotChocolate.Execution.Projections", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination.Extensions", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types", "16.4.0")
                            .WithNugetDependency("HotChocolate.Validation", "16.4.0")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "16.4.0"),
                        ( >= 8, >= 0) => new PackageVersion("16.4.0")
                            .WithNugetDependency("HotChocolate.Authorization", "16.4.0")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "16.4.0")
                            .WithNugetDependency("HotChocolate.Execution.Projections", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination.Extensions", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "16.4.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "16.4.0")
                            .WithNugetDependency("HotChocolate.Validation", "16.4.0"),
                        ( >= 7, >= 0) => new PackageVersion("14.3.1")
                            .WithNugetDependency("HotChocolate.Authorization", "14.3.1")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "14.3.1")
                            .WithNugetDependency("HotChocolate.Execution", "14.3.1")
                            .WithNugetDependency("HotChocolate.Fetching", "14.3.1")
                            .WithNugetDependency("HotChocolate.Pagination.Mappings", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.3.1")
                            .WithNugetDependency("HotChocolate.Validation", "14.3.1"),
                        ( >= 6, >= 0) => new PackageVersion("14.3.1")
                            .WithNugetDependency("HotChocolate.Authorization", "14.3.1")
                            .WithNugetDependency("HotChocolate.Execution", "14.3.1")
                            .WithNugetDependency("HotChocolate.Fetching", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.3.1")
                            .WithNugetDependency("HotChocolate.Validation", "14.3.1"),
                        ( >= 2, >= 0) => new PackageVersion("14.3.1")
                            .WithNugetDependency("HotChocolate.Authorization", "14.3.1")
                            .WithNugetDependency("HotChocolate.Execution", "14.3.1")
                            .WithNugetDependency("HotChocolate.Fetching", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.3.1")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.3.1")
                            .WithNugetDependency("HotChocolate.Validation", "14.3.1"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolatePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolate(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolatePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
