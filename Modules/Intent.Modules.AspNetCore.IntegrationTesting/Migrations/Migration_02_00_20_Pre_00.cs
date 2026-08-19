using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnVersionMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Migrations
{
    public class Migration_02_00_20_Pre_00 : IModuleMigration
    {
        private const string ModuleIdentifier = "Intent.AspNetCore.IntegrationTesting";
        private const string IntegrationTestSettingsGroupId = "d37f669a-8f2c-49ed-8c28-6ad31d836754";
        private const string IntegrationTestSettingsGroupTitle = "Integration Test Settings";
        private const string GenerateServiceProxiesForTestingSettingId = "1ba26513-e4ac-41f2-bba6-311b2cc2153b";
        private const string IntegrationTestGenerationModeSettingId = "1c5339c8-1f86-4058-af02-9fe200738fa3";

        private readonly IApplicationConfigurationProvider _configurationProvider;

        public Migration_02_00_20_Pre_00(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.IntegrationTesting";
        [IntentFully]
        public string ModuleVersion => "2.0.20-pre.0";

        /// <summary>
        /// Prior to this version, service proxies were always generated and integration test stubs were
        /// always scaffolded for every endpoint. Both behaviours are now controlled by settings whose
        /// defaults are aimed at new installs, so explicitly pin existing applications to the previous
        /// behaviour.
        /// </summary>
        public void Up()
        {
            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == IntegrationTestSettingsGroupId);
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = IntegrationTestSettingsGroupId,
                    Module = ModuleIdentifier,
                    Title = IntegrationTestSettingsGroupTitle,
                    Settings = []
                };
                app.ModuleSettingGroups.Add(group);
            }

            var changed = EnsureSetting(
                group,
                GenerateServiceProxiesForTestingSettingId,
                "Generate Service Proxies for Testing",
                ModuleSettingControlType.Switch,
                "true");

            changed |= EnsureSetting(
                group,
                IntegrationTestGenerationModeSettingId,
                "Integration Test Generation Mode",
                ModuleSettingControlType.Select,
                "all");

            if (changed)
            {
                app.SaveAllChanges();
            }
        }

        public void Down()
        {
        }

        private static bool EnsureSetting(
            ApplicationModuleSettingsPersistable group,
            string settingId,
            string title,
            ModuleSettingControlType controlType,
            string value)
        {
            if (group.Settings.Any(x => x.Id == settingId))
            {
                return false;
            }

            group.Settings.Add(new ModuleSettingPersistable
            {
                Id = settingId,
                Title = title,
                Module = ModuleIdentifier,
                ControlType = controlType,
                IsRequired = false,
                Value = value
            });

            return true;
        }
    }
}
