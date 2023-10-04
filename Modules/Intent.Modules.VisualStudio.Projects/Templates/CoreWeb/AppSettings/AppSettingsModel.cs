using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    public class AppSettingsModel
    {
        public AppSettingsModel(RuntimeEnvironmentModel runtimeEnvironment, string location, bool requiresSpecifiedRole)
        {
            RuntimeEnvironment = runtimeEnvironment;
            Location = location;
            RequiresSpecifiedRole = requiresSpecifiedRole;
        }

        public RuntimeEnvironmentModel RuntimeEnvironment { get; }

        public string Location { get; }

        public bool RequiresSpecifiedRole { get; }
    }
}