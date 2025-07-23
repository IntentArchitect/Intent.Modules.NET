using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.SqlServerImporter.Migrations
{
    public class Migration_01_02_01_Pre_02 : IModuleMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_01_02_01_Pre_02(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.SqlServerImporter";
        [IntentFully]
        public string ModuleVersion => "1.2.1-pre.2";

        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var packages = app.GetDesigners().SelectMany(x => x.GetPackages()).ToList();
            
            foreach (var package in packages)
            {
                bool changed = false;
                var metaEntries = package.Metadata.Where(p => p.Key.StartsWith("sql-import:") || p.Key.StartsWith("sql-import-repository:")).ToList();
                foreach (var metaEntry in metaEntries)
                {
                    var newKey = metaEntry.Key
                        .Replace("sql-import:", "rdbms-import:")
                        .Replace("sql-import-repository:", "rdbms-import-repository:");
                    if (newKey != metaEntry.Key)
                    {
                        metaEntry.Key = newKey;
                        changed = true;
                    }
                }

                if (changed)
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