using System.Linq;
using Intent.Modelers.CodebaseStructure.Api;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence.V2;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_04_00_01_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;
        private const string SqlServerDatabaseProjectStereotypeDefinitionId = "b8201720-882a-42ad-abb6-870a97815334";
        private const string ProjectTypePropertyId = "2099930c-69b8-4989-bb60-e2c252f8b7ac";
        private const string VersionPropertyId = "fcf08ff1-cd64-415e-9285-ed8b0de225ea";
        private const string VsPackageName = "Intent.VisualStudio.Projects";
        private const string VsPackageId = "a0636ab7-d3a1-430b-9609-11a18aa3cc7f";

        public Migration_04_00_01_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "4.0.1-pre.0";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();
            var designer = application.GetDesigner(ApiMetadataDesignerExtensions.CodebaseStructureDesignerId);

            foreach (var package in designer.GetPackages())
            {
                var requiresSave = false;

                foreach (var element in package.GetElementsOfType(SQLServerDatabaseProjectModel.SpecializationTypeId))
                {
                    if (element.Stereotypes.TryGet(SqlServerDatabaseProjectStereotypeDefinitionId, out _))
                    {
                        continue;
                    }

                    var stereotype = element.Stereotypes.Add(
                        definitionId: SqlServerDatabaseProjectStereotypeDefinitionId,
                        name: "SQL Server Database Project",
                        definitionPackageId: VsPackageId,
                        definitionPackageName: VsPackageName);

                    stereotype.AddedByDefault = true;

                    stereotype.Properties.Add(
                        id: ProjectTypePropertyId,
                        name: "Project Type",
                        value: ".NET Framework",
                        configure: config => { config.IsActive = true; });

                    stereotype.Properties.Add(
                        id: VersionPropertyId,
                        name: "Version",
                        value: "2.1.0",
                        configure: config => { config.IsActive = false; });

                    requiresSave = true;
                }

                if (requiresSave)
                {
                    package.Save();
                }
            }
        }

        public void Down()
        {
        }
    }
}