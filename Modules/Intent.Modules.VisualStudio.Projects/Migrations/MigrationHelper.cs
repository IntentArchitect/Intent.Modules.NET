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

        // Honoured for every SDK: ApplyGenerateSettingsDefaults turns it on for host projects first
        return generateLaunchSettings &&
               (sdkOptions.IsMicrosoftNETSdkWeb() || sdkOptions.IsMicrosoftNETSdkBlazorWebAssembly() || sdkOptions.IsMicrosoftNETSdkWorker() || sdkOptions.IsMicrosoftNETSdk());
    }

    /// <summary>
    /// Defaults "Generate AppSettings File" and "Generate LaunchSettings File" to true on every
    /// <c>C# Project (.NET)</c> with a host SDK (Web, Worker, Blazor WebAssembly), and records that SDK
    /// in the element's metadata. This mirrors the <c>.NET Settings</c> On Changed script, which uses the
    /// same marker to tell a default it applied from a value the user chose.
    /// </summary>
    public static void ApplyGenerateSettingsDefaults(IApplicationPersistable application)
    {
        foreach (var package in application.GetDesigners().Where(s => s.Id == Designers.VisualStudioId).SelectMany(x => x.GetPackages()))
        {
            var requiresSave = false;

            foreach (var project in package.GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId))
            {
                if (!project.Stereotypes.TryGet(NETSettings.DefinitionId, out var netSettings) ||
                    !netSettings.Properties.TryGet(SdkPropertyId, out var sdkProperty) ||
                    !HostSdks.Contains(sdkProperty.Value))
                {
                    continue;
                }

                SetFlag(netSettings, GenerateAppSettingsFilePropertyId, "Generate AppSettings File");
                SetFlag(netSettings, GenerateLaunchSettingsFilePropertyId, "Generate LaunchSettings File");

                if (project.Metadata.Exists(GenerateSettingsDefaultsSdkMetadataKey))
                {
                    project.Metadata.Update(GenerateSettingsDefaultsSdkMetadataKey, sdkProperty.Value);
                }
                else
                {
                    project.Metadata.Add(GenerateSettingsDefaultsSdkMetadataKey, sdkProperty.Value);
                }

                requiresSave = true;
            }

            if (requiresSave)
            {
                package.Save();
            }
        }

        static void SetFlag(Persistence.IStereotypePersistable netSettings, string propertyId, string propertyName)
        {
            if (!netSettings.Properties.TryGet(propertyId, out var property))
            {
                property = netSettings.Properties.Add(
                    id: propertyId,
                    name: propertyName,
                    value: "true",
                    configure: config => config.IsActive = true);
            }

            property.Value = "true";
            property.IsActive = true;
        }
    }

    /// <summary>
    /// Must match the key the <c>.NET Settings</c> On Changed script uses.
    /// </summary>
    private const string GenerateSettingsDefaultsSdkMetadataKey = "generate-settings-defaults-sdk";
    private const string SdkPropertyId = "1f0cdbc4-7a18-40a5-aeca-90452cce4fcf";
    private const string GenerateAppSettingsFilePropertyId = "a811d3f2-253e-46de-ae72-bd6ec9f30d54";
    private const string GenerateLaunchSettingsFilePropertyId = "4fe561b9-333e-4341-8243-d569db8cd8f4";

    private static readonly string[] HostSdks =
    [
        "Microsoft.NET.Sdk.Web",
        "Microsoft.NET.Sdk.Worker",
        "Microsoft.NET.Sdk.BlazorWebAssembly"
    ];

    public static string GetNewBaseUrl()
    {
        var portNumber = Random.Shared.Next(44300, 44399);
        return $"https://localhost:{portNumber}/";
    }
}
