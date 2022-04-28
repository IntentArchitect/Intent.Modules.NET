using Intent.SdkEvolutionHelpers;

namespace Intent.Modules.VisualStudio.Projects.Events
{
    [FixFor_Version4] // Try get rid of this, can't actually see what's using it.
    public class CsProjectEvents
    {
        public const string AddImport = "SoftwareFactoryEvents.CsProject.AddImport";
        public const string AddCompileDependsOn = "SoftwareFactoryEvents.CsProject.AddCompileDependsOn";
        public const string AddBeforeBuild = "SoftwareFactoryEvents.CsProject.AddBeforeBuild";
        public const string AddContentFile = "SoftwareFactoryEvents.CsProject.AddContentFile";
    }
}
