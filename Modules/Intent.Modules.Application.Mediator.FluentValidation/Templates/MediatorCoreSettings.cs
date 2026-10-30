using Intent.Engine;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Application.Mediator.FluentValidation.Templates
{
    /// <summary>
    /// Reads the core module's "Mediator Settings" group. The core project is referenced with
    /// <c>PrivateAssets=All</c>, so only its inlined constants are safe to use here, not its generated
    /// <c>GetMediatorSettings()</c> accessor. These ids must match the core's
    /// <c>Settings/ModuleSettingsExtensions.cs</c>.
    /// </summary>
    internal static class MediatorCoreSettings
    {
        private const string MediatorSettingsGroupId = "d87f0cfc-b32a-4297-aec4-46ec6944b77f";
        private const string ConsolidateIntoSingleFileSettingId = "f36cb70b-3d0a-4562-b734-f3efe4dc6a95";

        public static bool ConsolidateIntoSingleFile(this ISoftwareFactoryExecutionContext executionContext)
        {
            return executionContext.Settings.ConsolidateIntoSingleFile();
        }

        public static bool ConsolidateIntoSingleFile(this IApplicationSettingsProvider settings)
        {
            var value = settings.GetGroup(MediatorSettingsGroupId)?.GetSetting(ConsolidateIntoSingleFileSettingId)?.Value;
            return bool.TryParse(value?.ToPascalCase(), out var result) && result;
        }
    }
}
