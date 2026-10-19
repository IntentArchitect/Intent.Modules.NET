using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    public class AppSettingsModel
    {
        public AppSettingsModel(RuntimeEnvironmentModel runtimeEnvironment, string location, bool requiresSpecifiedRole, bool includeAllowHosts, bool includeAspNetCoreLoggingLevel, bool isHandsOff = false)
        {
            RuntimeEnvironment = runtimeEnvironment;
            Location = location;
            RequiresSpecifiedRole = requiresSpecifiedRole;
            IncludeAllowHosts = includeAllowHosts;
            IncludeAspNetCoreLoggingLevel = includeAspNetCoreLoggingLevel;
            IsHandsOff = isHandsOff;
        }

        public RuntimeEnvironmentModel RuntimeEnvironment { get; }

        public string Location { get; }

        public bool RequiresSpecifiedRole { get; }

        public bool IncludeAllowHosts { get; }

        public bool IncludeAspNetCoreLoggingLevel { get; }

        /// <summary>
        /// <c>Generate AppSettings File</c> is off for a host project: an existing file is left exactly
        /// as it is, none is created, and settings raised for the project are dropped rather than written.
        /// </summary>
        public bool IsHandsOff { get; }
    }
}
