using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    public class AppSettingsModel
    {
        public AppSettingsModel(RuntimeEnvironmentModel runtimeEnvironment, string location, bool requiresSpecifiedRole, bool includeAllowHosts, bool includeAspNetCoreLoggingLevel)
        {
            RuntimeEnvironment = runtimeEnvironment;
            Location = location;
            RequiresSpecifiedRole = requiresSpecifiedRole;
            IncludeAllowHosts = includeAllowHosts;
            IncludeAspNetCoreLoggingLevel = includeAspNetCoreLoggingLevel;
		}

        public RuntimeEnvironmentModel RuntimeEnvironment { get; }

        public string Location { get; }

        public bool RequiresSpecifiedRole { get; }

        public bool IncludeAllowHosts { get; }
		public bool IncludeAspNetCoreLoggingLevel { get; }

	}
}