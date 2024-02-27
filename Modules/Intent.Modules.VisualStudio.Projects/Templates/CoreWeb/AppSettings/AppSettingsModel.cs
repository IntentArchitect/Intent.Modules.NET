using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    public class AppSettingsModel
    {
        public AppSettingsModel(RuntimeEnvironmentModel runtimeEnvironment, string location, bool requiresSpecifiedRole, bool includeAllowHosts = true)
        {
            RuntimeEnvironment = runtimeEnvironment;
            Location = location;
            RequiresSpecifiedRole = requiresSpecifiedRole;
            IncludeAllowHosts = includeAllowHosts;
		}

        public RuntimeEnvironmentModel RuntimeEnvironment { get; }

        public string Location { get; }

        public bool RequiresSpecifiedRole { get; }

        public bool IncludeAllowHosts { get; }

	}
}