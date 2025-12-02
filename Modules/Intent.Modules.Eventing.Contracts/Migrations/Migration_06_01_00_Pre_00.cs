using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.Eventing.Contracts.Migrations
{
    public class Migration_06_01_00_Pre_00 : IModuleMigration
    {
        public Migration_06_01_00_Pre_00()
        {
        }

        [IntentFully]
        public string ModuleId => "Intent.Eventing.Contracts";
        [IntentFully]
        public string ModuleVersion => "6.1.0-pre.0";

        public void Up()
        {
        }

        public void Down()
        {
        }
    }
}