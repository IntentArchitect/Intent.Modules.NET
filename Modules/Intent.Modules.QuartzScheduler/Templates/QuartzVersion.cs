using System.Linq;
using System.Text.RegularExpressions;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.QuartzScheduler.Templates;

internal static class QuartzVersion
{
    // Quartz 4 ships for net10.0 only, so a project that also targets an earlier framework stays on Quartz 3.
    public static bool UsesQuartz4(IOutputTarget outputTarget)
    {
        var frameworks = outputTarget.GetProject().GetSupportedFrameworks().ToArray();
        return frameworks.Length > 0 && frameworks.All(IsNet10OrLater);
    }

    // The framework this module's NuGet packages resolve against: the project's highest, except where that row is
    // Quartz 4 and the project also targets an earlier framework. .NET 9 is then the newest row still on Quartz 3.
    public static MajorMinorVersion GetPackageFramework(IOutputTarget outputTarget)
    {
        var maxVersion = outputTarget.GetMaxNetAppVersion();
        return maxVersion.Major >= 10 && !UsesQuartz4(outputTarget)
            ? MajorMinorVersion.Create(9, 0)
            : maxVersion;
    }

    private static bool IsNet10OrLater(string framework)
    {
        var match = Regex.Match(framework, @"^net(?<major>\d+)\.\d+");
        return match.Success && int.Parse(match.Groups["major"].Value) >= 10;
    }
}
