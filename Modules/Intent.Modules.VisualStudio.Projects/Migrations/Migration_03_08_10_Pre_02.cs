using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using System.Linq;
using static Intent.Modules.VisualStudio.Projects.Api.VisualStudioSolutionModelStereotypeExtensions;
using System.Diagnostics;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_03_08_10_Pre_02 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_03_08_10_Pre_02(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "3.8.10-pre.2";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ApiMetadataDesignerExtensions.VisualStudioDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                if(!package.Stereotypes.Any(s => s.DefinitionId == VisualStudioSolutionOptions.DefinitionId))
                {
                    package.Stereotypes.Add(new StereotypePersistable
                    {
                        DefinitionId = VisualStudioSolutionOptions.DefinitionId,
                        Name = "Visual Studio Solution Options",
                        DefinitionPackageName = "Intent.VisualStudio.Projects",
                        DefinitionPackageId = "a0636ab7-d3a1-430b-9609-11a18aa3cc7f"
                    });
                }

                var vsOptionsStereotype = package.Stereotypes.First(s => s.DefinitionId == VisualStudioSolutionOptions.DefinitionId);

                var cpmEnabled = GetOrCreateProperty(vsOptionsStereotype, "49e78350-c77b-4f83-88d8-5841e6cb3", "Manage Package Versions Centrally", true, "false");
                GetOrCreateProperty(vsOptionsStereotype, "ad4bb70e-9dbd-41ce-95a0-1d319a4", "Output Location", bool.Parse(cpmEnabled?.Value), "Check Parent Folders");
                GetOrCreateProperty(vsOptionsStereotype, "ca30802f-7cf3-459e-9ea1-aa20d7af763", "Only check current Git repository", bool.Parse(cpmEnabled?.Value), "true");

                package.Save(true);
            }
        }

        private static StereotypePropertyPersistable GetOrCreateProperty(StereotypePersistable settings, string id, string name, bool isActive, string value = "")
        {
            var property = settings.Properties.SingleOrDefault(x => x.Name == name);
            if (property == null)
            {
                property = new StereotypePropertyPersistable
                {
                    DefinitionId = id,
                    Name = name,
                    IsActive = isActive,
                    Value = string.IsNullOrEmpty(value) ? null : value
                };
                settings.Properties.Add(property);
            }

            return property;
        }

        public void Down()
        {
        }
    }
}