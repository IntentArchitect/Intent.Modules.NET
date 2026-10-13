using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Intent.Modules.VisualStudio.Projects.Templates;

/// <summary>
/// Brings a project's <c>Sdk</c> attribute in line with the <c>.NET Settings</c> SDK, without
/// overwriting an SDK the model can't represent (e.g. <c>Microsoft.Build.NoTargets/3.7.0</c>).
/// </summary>
internal static class SdkAttributeSync
{
    private const string AspireAppHostSdk = "Aspire.AppHost.Sdk";
    private const string MSTestSdk = "MSTest.Sdk";

    /// <summary>
    /// The <c>.NET Settings</c> SDK options. An existing SDK outside this set is never rewritten.
    /// </summary>
    private static readonly HashSet<string> RepresentableSdks = new(StringComparer.OrdinalIgnoreCase)
    {
        "Microsoft.NET.Sdk",
        "Microsoft.NET.Sdk.Web",
        "Microsoft.NET.Sdk.BlazorWebAssembly",
        "Microsoft.NET.Sdk.Razor",
        "Microsoft.NET.Sdk.Worker",
        "Microsoft.NET.Sdk.WindowsDesktop",
        AspireAppHostSdk,
        MSTestSdk
    };

    /// <returns>True if the attribute was changed.</returns>
    public static bool Sync(XDocument doc, string modelledSdk, string aspireSdkVersion, string msTestSdkVersion)
    {
        var attribute = doc.Root?.Attribute("Sdk");
        if (attribute == null || string.IsNullOrWhiteSpace(modelledSdk))
        {
            return false;
        }

        var existingValue = attribute.Value;
        var existingName = GetSdkName(existingValue);
        if (!RepresentableSdks.Contains(existingName))
        {
            return false;
        }

        var value = GetModelledValue(modelledSdk, aspireSdkVersion, msTestSdkVersion, existingName, existingValue);
        if (value == existingValue)
        {
            return false;
        }

        attribute.Value = value;
        return true;
    }

    /// <summary>
    /// The SDK name without any <c>/version</c> suffix, e.g. <c>MSTest.Sdk/4.0.2</c> becomes <c>MSTest.Sdk</c>.
    /// </summary>
    public static string GetSdkName(string sdkValue)
    {
        if (sdkValue == null)
        {
            return null;
        }

        var separatorIndex = sdkValue.IndexOf('/');
        return (separatorIndex < 0 ? sdkValue : sdkValue[..separatorIndex]).Trim();
    }

    private static string GetModelledValue(string modelledSdk, string aspireSdkVersion, string msTestSdkVersion, string existingName, string existingValue)
    {
        if (modelledSdk == AspireAppHostSdk)
        {
            return $"{modelledSdk}/{aspireSdkVersion}";
        }

        if (modelledSdk == MSTestSdk)
        {
            if (!string.IsNullOrWhiteSpace(msTestSdkVersion))
            {
                return $"{modelledSdk}/{msTestSdkVersion.Trim()}";
            }

            // No modelled version: keep any version already written, otherwise global.json resolves it.
            return string.Equals(existingName, MSTestSdk, StringComparison.OrdinalIgnoreCase)
                ? existingValue
                : modelledSdk;
        }

        return modelledSdk;
    }
}
