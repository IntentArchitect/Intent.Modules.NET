using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Metadata.Models;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Persistence;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions;
using static Intent.Modules.VisualStudio.Projects.Api.CSharpProjectNETModelStereotypeExtensions.NETSettings;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.VisualStudio.Projects.Migrations
{
    public class Migration_03_09_02_Pre_00 : IModuleMigration
    {
        private readonly IPersistenceLoader _persistenceLoader;

        public Migration_03_09_02_Pre_00(IPersistenceLoader persistenceLoader)
        {
            _persistenceLoader = persistenceLoader;
        }

        [IntentFully]
        public string ModuleId => "Intent.VisualStudio.Projects";
        [IntentFully]
        public string ModuleVersion => "3.9.2-pre.0";

        public void Up()
        {
            var application = _persistenceLoader.LoadCurrentApplication();

            MigrationHelper.ApplyLaunchSettingsStereotype(application);
        }

        public void Down()
        {
            var application = _persistenceLoader.LoadCurrentApplication();

            foreach (var package in application.GetDesigners().Where(s => s.Id == Designers.VisualStudioId).SelectMany(x => x.GetPackages()))
            {
                bool requiresSave = false;

                foreach (var element in
                    package.GetElementsOfType(ASPNETCoreWebApplicationModel.SpecializationTypeId)
                        .Union(package.GetElementsOfType(CSharpProjectNETModel.SpecializationTypeId))
                    .Where(c => c.Stereotypes.Any(s => s.DefinitionId == LaunchSettings.DefinitionId)))
                {
                    var stereotype = element.Stereotypes.FirstOrDefault(s => s.DefinitionId == LaunchSettings.DefinitionId);
                    if (stereotype != null)
                    {
                        element.Stereotypes.Remove(stereotype);
                        requiresSave = true;
                    }
                }

                if (requiresSave)
                {
                    package.Save();
                }
            }
        }
    }
}