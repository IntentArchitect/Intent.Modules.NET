using Intent.Modules.AzureFunctions.Settings;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AzureFunctions.Templates.AzureFunctionClass.TriggerStrategies;

/// <summary>
/// Applies the "Trigger Names From App Settings" module setting consistently across trigger handlers.
/// </summary>
internal static class TriggerAppSettings
{
    public static bool IsEnabled(AzureFunctionClassTemplate template)
    {
        return template.ExecutionContext.Settings.GetAzureFunctionsSettings().TriggerNamesFromAppSettings();
    }

    /// <summary>
    /// Returns the quoted C# string for a trigger binding value. When the setting is enabled, a non-blank value that
    /// isn't already a binding expression (e.g. "%My:Queue%") becomes a reference to the per-function app setting
    /// <c>AzureFunctions:{FunctionName}:{part}</c>, seeded into local.settings.json with the modelled value.
    /// </summary>
    public static string GetBindingValue(AzureFunctionClassTemplate template, string part, string value)
    {
        if (!IsEnabled(template) || string.IsNullOrWhiteSpace(value) || IsBindingExpression(value))
        {
            return $@"""{value}""";
        }

        var key = $"AzureFunctions:{template.GetFunctionName()}:{part}";
        template.ApplyAppSetting(key, value);
        return $@"""%{key}%""";
    }

    /// <summary>
    /// When the setting is enabled, seeds an empty entry for a connection setting name so local.settings.json lists
    /// every setting the function needs. An existing entry is left untouched.
    /// </summary>
    public static void SeedConnection(AzureFunctionClassTemplate template, string connection)
    {
        if (!IsEnabled(template) || string.IsNullOrWhiteSpace(connection))
        {
            return;
        }

        template.ApplyAppSetting(connection, "");
    }

    private static bool IsBindingExpression(string value)
    {
        return value.Length > 1 && value.StartsWith('%') && value.EndsWith('%');
    }
}
