using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.AppSettings
{
    public class AppSettingsModel
    {
        public AppSettingsModel(RuntimeEnvironmentModel runtimeEnvironment = null)
        {
            RuntimeEnvironment = runtimeEnvironment;
        }

        public RuntimeEnvironmentModel RuntimeEnvironment { get; }
    }
}