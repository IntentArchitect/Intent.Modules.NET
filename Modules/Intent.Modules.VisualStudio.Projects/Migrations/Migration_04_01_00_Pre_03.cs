using System.Linq;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_04_01_00_Pre_03 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_04_01_00_Pre_03(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "4.1.0-pre.3";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);

            var packages = designer.GetPackages().ToArray();

            foreach (var package in designer.GetPackages())
            {
                package.SpecializationType = RootFolderModel.SpecializationType;
                package.SpecializationTypeId = RootFolderModel.SpecializationTypeId;

                package.Save();
            }
        }

        public void Down()
        {
        }
    }
}