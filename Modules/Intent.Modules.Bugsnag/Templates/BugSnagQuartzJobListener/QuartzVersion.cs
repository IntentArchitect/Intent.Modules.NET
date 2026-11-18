using System.Linq;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;
using NuGet.Versioning;

namespace Intent.Modules.Bugsnag.Templates.BugSnagQuartzJobListener;

internal static class QuartzVersion
{
    // Decided from the Quartz package Intent.QuartzScheduler's configuration template requested, so the listener matches
    // the Quartz that project gets, whichever version of that module is installed and whatever it targets.
    public static bool UsesQuartz4(ITemplate quartzConfigurationTemplate)
    {
        return quartzConfigurationTemplate is IHasNugetDependencies template &&
            template.GetNugetDependencies().Any(package =>
                package.Name.StartsWith("Quartz") &&
                NuGetVersion.TryParse(package.Version, out var version) &&
                version.Major >= 4);
    }
}
