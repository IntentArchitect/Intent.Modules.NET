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
                        ( >= 9, >= 0) => new PackageVersion("15.1.11")
                            .WithNugetDependency("HotChocolate.Authorization", "15.1.11")
                            .WithNugetDependency("HotChocolate.Execution.Projections", "15.1.11")
                            .WithNugetDependency("HotChocolate.Execution", "15.1.11")
                            .WithNugetDependency("HotChocolate.Fetching", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination.Extensions", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.Queries", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types", "15.1.11")
                            .WithNugetDependency("HotChocolate.Validation", "15.1.11")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "15.1.11"),
                        ( >= 8, >= 0) => new PackageVersion("15.1.11")
                            .WithNugetDependency("HotChocolate.Authorization", "15.1.11")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "15.1.11")
                            .WithNugetDependency("HotChocolate.Execution", "15.1.11")
                            .WithNugetDependency("HotChocolate.Execution.Projections", "15.1.11")
                            .WithNugetDependency("HotChocolate.Fetching", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination.Extensions", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "15.1.11")
                            .WithNugetDependency("HotChocolate.Types.Queries", "15.1.11")
                            .WithNugetDependency("HotChocolate.Validation", "15.1.11"),
                        ( >= 7, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.3.0")
                            .WithNugetDependency("HotChocolate.CostAnalysis", "14.3.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.3.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.3.0")
                            .WithNugetDependency("HotChocolate.Pagination.Mappings", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.3.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.3.0"),
                        ( >= 6, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.3.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.3.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.3.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.3.0"),
                        ( >= 2, >= 0) => new PackageVersion("14.3.0")
                            .WithNugetDependency("HotChocolate.Authorization", "14.3.0")
                            .WithNugetDependency("HotChocolate.Execution", "14.3.0")
                            .WithNugetDependency("HotChocolate.Fetching", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.CursorPagination", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Mutations", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.OffsetPagination", "14.3.0")
                            .WithNugetDependency("HotChocolate.Types.Queries", "14.3.0")
                            .WithNugetDependency("HotChocolate.Validation", "14.3.0"),
                        _ => throw new Exception($"Unsupported Framework `{framework.Major}` for NuGet package '{HotChocolatePackageName}'"),
                    }
                );
        }

        public static NugetPackageInfo HotChocolate(IOutputTarget outputTarget) => NugetRegistry.GetVersion(HotChocolatePackageName, outputTarget.GetMaxNetAppVersion());
    }
}
