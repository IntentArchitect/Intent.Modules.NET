using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Wasm.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        public OnInstallMigration()
        {
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor.Wasm";

        public void OnInstall()
        {
        }
    }
}