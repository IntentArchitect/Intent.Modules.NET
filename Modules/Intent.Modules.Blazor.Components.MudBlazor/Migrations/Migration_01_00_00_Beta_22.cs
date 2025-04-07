using Intent.Blazor.Components.MudBlazor.Api;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.IArchitect.Agent.Persistence.Model.Common;
using Intent.Modelers.UI.Api;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;
using System.Linq;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Components.MudBlazor.Migrations
{
    public class Migration_01_00_00_Beta_22 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_01_00_00_Beta_22(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor.Components.MudBlazor";
        [IntentFully]
        public string ModuleVersion => "1.0.0-beta.22";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);
            var designer = app.GetDesigner(ApiMetadataDesignerExtensions.UserInterfaceDesignerId);
            var packages = designer.GetPackages();

            foreach (var package in packages)
            {
                bool hasChanges = false;
                package.Load();
                var datagrids = package.GetElementsOfType(DataGridModel.SpecializationTypeId);

                //Check if Stereotype doesn't already exist
                foreach (var datagrid in datagrids)
                {
                    if (!datagrid.Stereotypes.Any(s => s.DefinitionId == DataGridModelStereotypeExtensions.Appearance.DefinitionId))
                    {
                        var stereotype = new StereotypePersistable
                        {
                            DefinitionId = DataGridModelStereotypeExtensions.Appearance.DefinitionId,
                            Name = "Appearance",
                            DefinitionPackageName = "Intent.Blazor.Components.MudBlazor",
                            DefinitionPackageId = "47e5f8d0-3892-4408-b6f9-88bf8591af2d",
                            Properties = new System.Collections.Generic.List<StereotypePropertyPersistable>{ new StereotypePropertyPersistable
                                {
                                
                                    DefinitionId = "696f0b1c-bcfb-43c1-a490-3d76451ade04",
                                    Name = "Hover",
                                    Value = "true",
                                    IsActive = true
                                }}
                        };
                        datagrid.Stereotypes.Add(stereotype);
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                {
                    package.Save(true);
                }
            }
        }

        public void Down()
        {
        }
    }
}