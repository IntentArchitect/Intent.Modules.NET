using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Modelers.Domain.Api;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.SoftDelete.Migrations
{
    public class Migration_01_00_04_Pre_01 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_01_00_04_Pre_01(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.EntityFrameworkCore.SoftDelete";
        [IntentFully]
        public string ModuleVersion => "1.0.4-pre.1";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigner(ApiMetadataDesignerExtensions.DomainDesignerId).GetPackages();

            foreach (var package in packages)
            {
                var attributes = package
                    .GetElementsOfType(AttributeModel.SpecializationTypeId)
                    .Where(x => x.Metadata.Any(x => x.Key == "soft-delete" && x.Value == "true"))
                    .ToArray();

                foreach (var attribute in attributes)
                {
                    attribute.Metadata.Add(new GenericMetadataPersistable
                    {
                        Key = "set-by-infrastructure",
                        Value = "true"
                    });
                }

                if (attributes.Length > 0)
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