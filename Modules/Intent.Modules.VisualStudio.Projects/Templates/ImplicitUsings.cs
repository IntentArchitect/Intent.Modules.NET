using System.Collections.Generic;

namespace Intent.Modules.VisualStudio.Projects.Templates;

/// <summary>
/// https://learn.microsoft.com/en-gb/dotnet/core/project-sdk/overview#implicit-using-directives
/// </summary>
internal static class ImplicitUsings
{
    public static readonly IReadOnlyCollection<string> ForSdk =
    [
        "System",
        "System.Collections.Generic",
        "System.IO",
        "System.Linq",
        "System.Net.Http",
        "System.Threading",
        "System.Threading.Tasks"
    ];

    // MSTest.Sdk imports Microsoft.NET.Sdk, and its MSTest.TestFramework reference adds the MSTest
    // namespace when ImplicitUsings is enabled (buildTransitive/MSTest.TestFramework.targets, 4.0.2).
    public static readonly IReadOnlyCollection<string> ForMSTestSdk =
    [
        "System",
        "System.Collections.Generic",
        "System.IO",
        "System.Linq",
        "System.Net.Http",
        "System.Threading",
        "System.Threading.Tasks",
        "Microsoft.VisualStudio.TestTools.UnitTesting"
    ];

    public static readonly IReadOnlyCollection<string> ForBlazorWebAssemblySdk =
    [
        "System",
        "System.Collections.Generic",
        "System.IO",
        "System.Linq",
        "System.Net.Http",
        "System.Threading",
        "System.Threading.Tasks",
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Logging"
    ];

    // Worked out by looking at the obj/<configuration>/<target>/<assembly-name>.GlobalUsings.g.cs file
    public static readonly IReadOnlyCollection<string> ForWebSdk =
    [
        "System",
        "System.Collections.Generic",
        "System.IO",
        "System.Linq",
        "System.Net.Http",
        "System.Threading",
        "System.Threading.Tasks",
        "System.Net.Http.Json",
        "Microsoft.AspNetCore.Builder",
        "Microsoft.AspNetCore.Hosting",
        "Microsoft.AspNetCore.Http",
        "Microsoft.AspNetCore.Routing",
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Hosting",
        "Microsoft.Extensions.Logging"
    ];

    public static readonly IReadOnlyCollection<string> ForWorkerSdk =
    [
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Hosting",
        "Microsoft.Extensions.Logging"
    ];
}