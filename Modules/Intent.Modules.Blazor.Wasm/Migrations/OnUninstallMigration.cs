using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnUninstallMigration", Version = "1.0")]

namespace Intent.Modules.Blazor.Wasm.Migrations
{
    public class OnUninstallMigration : IModuleOnUninstallMigration
    {
        public OnUninstallMigration()
        {
        }

        [IntentFully]
        public string ModuleId => "Intent.Blazor.Wasm";

        public void OnUninstall()
        {
        }
    }
}