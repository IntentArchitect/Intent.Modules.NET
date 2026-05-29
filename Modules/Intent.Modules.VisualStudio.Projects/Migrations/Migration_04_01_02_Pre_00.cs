using System.Diagnostics;
using System.Linq;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_04_01_02_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        private const string VisualStudioSolutionSpecializationId = "769a45a2-119f-434f-8c27-bd4a399b915c";
        private const string FolderSpecializationId = "4d95d53a-8855-4f35-aa82-e312643f5c5f";


        public Migration_04_01_02_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "4.1.2-pre.0";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);

            var packages = designer.GetPackages().ToArray();

            foreach (var package in designer.GetPackages())
            {
                // get the vsSolution
                var vsSolution = package.Classes.FirstOrDefault(e => e.SpecializationTypeId == VisualStudioSolutionSpecializationId);
                if (vsSolution is null)
                {
                    continue;
                }

                // is there an .agent folder under the vs solution? This is incorrect, and needs to be moved to under the root
                var agentFolder = package.Classes.FirstOrDefault(e => e.SpecializationTypeId == FolderSpecializationId
                    && e.Name == ".agents"
                    && e.ParentFolderId == vsSolution.Id);

                if (agentFolder is null)
                {
                    continue;
                }

                agentFolder.ParentFolderId = package.Id;

                package.Save();
            }
        }

        public void Down()
        {
        }
    }
}