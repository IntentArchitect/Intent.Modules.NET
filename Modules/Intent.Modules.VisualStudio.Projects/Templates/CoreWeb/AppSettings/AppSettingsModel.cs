using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    public class AppSettingsModel
    {
        public AppSettingsModel(ASPNETCoreWebApplicationModel project, RuntimeEnvironmentModel runtimeEnvironment = null)
        {
            Project = project;
            RuntimeEnvironment = runtimeEnvironment;
        }

        public ASPNETCoreWebApplicationModel Project { get; }

        public RuntimeEnvironmentModel RuntimeEnvironment { get; }
    }
}