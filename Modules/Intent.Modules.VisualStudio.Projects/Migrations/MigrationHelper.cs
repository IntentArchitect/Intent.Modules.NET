using System;
using System.Linq;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions.NETSettings;

namespace Intent.Modules.VisualStudio.Projects.Migrations;

internal static class MigrationHelper
{
    public static void ApplyLaunchSettingsStereotype(IApplicationPersistable application, bool setValue = false)
    {
        foreach (var package in application.GetDesigners().Where(s => s.Id == Designers.VisualStudioId).SelectMany(x => x.GetPackages()))
        {
            var requiresSave = false;

            var projects = package.GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
                .Select(x => x)
                .Union(
                    package.GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId)
                        .Where(HasLaunchSettings)
                        .Select(x => x));

            foreach (var project in projects)
            {
                if (!project.Stereotypes.TryGet(LaunchSettings.DefinitionId, out var launchSettings))
                {
                    launchSettings = project.Stereotypes.Add(
                        definitionId: ProjectStereotypes.LaunchSettings.Id,
                        name: "Launch Settings",
                        definitionPackageId: "a0636ab7-d3a1-430b-9609-11a18aa3cc7f",
                        definitionPackageName: "Intent.VisualStudio.Projects");
                }

                if (!launchSettings!.Properties.TryGet(ProjectStereotypes.LaunchSettings.BaseUrlId, out var property))
                {
                    property = launchSettings.Properties.Add(
                        id: ProjectStereotypes.LaunchSettings.BaseUrlId,
                        name: "Base URL",
                        value: "",
                        configure: config =>
                        {
                            config.IsActive = true;
                        });
                }

                if (setValue && string.IsNullOrWhiteSpace(property!.Value))
                {
                    property.Value = GetNewBaseUrl();
                    requiresSave = true;
                }
            }

            if (requiresSave)
            {
                package.Save();
            }
        }
    }

    public static bool HasLaunchSettings(Persistence.IElementPersistable element)
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
        var portNumber = Random.Shared.Next(44300, 44399);
        return $"https://localhost:{portNumber}/";
    }
}
