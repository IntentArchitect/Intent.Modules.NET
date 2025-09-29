using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions.NETSettings;

namespace Intent.Modules.VisualStudio.Projects.Migrations;
internal static class MigrationHelper
{
    public static void ApplyLaunchSettingsStereotype(IApplicationPersistable application, bool setValue = false)
    {
        foreach (var package in application.GetDesigners().Where(s => s.Id == Designers.VisualStudioId).SelectMany(x => x.GetPackages()))
        {
            bool requiresSave = false;

            var projects = package.GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
                .Select(x => x)
                .Union(
                    package.GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId)
                    .Where(x => GetLaunchSettings(x))
                    .Select(x => x));

            foreach (var project in projects)
            {
                if (project.Stereotypes.Any(s => s.DefinitionId == LaunchSettings.DefinitionId))
                {
                    continue;
                }

                project.Stereotypes.Add(ProjectStereotypes.LaunchSettings.Id, "Launch Settings",
                    "a0636ab7-d3a1-430b-9609-11a18aa3cc7f", "Intent.VisualStudio.Projects", config =>
                    {
                        config.Properties.Add(ProjectStereotypes.LaunchSettings.BaseUrlId, "Base URL", setValue ? GetNewBaseUrl() : "", propConfig =>
                        {
                            propConfig.IsActive = true;
                        });
                    });

                requiresSave = true;
            }

            if (requiresSave)
            {
                package.Save();
            }
        }
    }

    public static bool GetLaunchSettings(Persistence.IElementPersistable element)
    {
        if (!element.Stereotypes.Any(s => s.Name == ".NET Settings"))
        {
            return false;
        }

        var netSettings = element.Stereotypes.FirstOrDefault(s => s.Name == ".NET Settings");

        if (netSettings is null || !netSettings.Properties.Any(p => p.Name == "SDK"))
        {
            return false;
        }

        var sdkString = netSettings.Properties.FirstOrDefault(p => p.Name == "SDK")?.Value;
        if (string.IsNullOrWhiteSpace(sdkString))
        {
            return false;
        }

        _ = bool.TryParse(netSettings.Properties.FirstOrDefault(p => p.Name == "Generate LaunchSettings File")?.Value, out var generateLaunchSettings);

        var sdkOptions = new SDKOptions(sdkString);

        return sdkOptions.IsMicrosoftNETSdkWeb() || sdkOptions.IsMicrosoftNETSdkBlazorWebAssembly() || sdkOptions.IsMicrosoftNETSdkWorker()
            || (sdkOptions.IsMicrosoftNETSdk() && generateLaunchSettings);
    }

    public static string GetNewBaseUrl()
    {
        var portNumber = new Random().Next(44300, 44399);
        return $"https://localhost:{portNumber}/";
    }
}
