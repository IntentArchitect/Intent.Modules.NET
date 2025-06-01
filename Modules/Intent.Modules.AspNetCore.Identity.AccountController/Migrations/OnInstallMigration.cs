using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.IArchitect.Agent.Persistence.Model;
using Intent.Plugins;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Migrations.OnInstallMigration", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Identity.AccountController.Migrations
{
    public class OnInstallMigration : IModuleOnInstallMigration
    {
        private readonly IApplicationConfigurationProvider _configurationProvider;

        public OnInstallMigration(IApplicationConfigurationProvider configurationProvider)
        {
            _configurationProvider = configurationProvider;
        }

        [IntentFully]
        public string ModuleId => "Intent.AspNetCore.Identity.AccountController";

        public void OnInstall()
        {

            var app = ApplicationPersistable.Load(_configurationProvider.GetApplicationConfig().FilePath);

            if (app.Modules.Any(m => m.ModuleId == "Intent.ModularMonolith.Module"))
            {
                app = null;
                var solution = SolutionPersistable.Load(Path.Combine(_configurationProvider.GetSolutionConfig().SolutionRootLocation, _configurationProvider.GetSolutionConfig().SolutionName + ".isln"));
                foreach (var current in solution.GetApplications())
                {
                    if (current.Modules.Any(m => m.ModuleId == "Intent.ModularMonolith.Host"))
                    {
                        app = current;
                        break;
                    }
                }
                if (app is null)
                {
                    throw new System.Exception($"Unable to find Modular Monolith Host for : {app.Name} (Application)");
                }
            }

            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "2c1a1918-97eb-4b21-8a15-021dd72db73c");
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "2c1a1918-97eb-4b21-8a15-021dd72db73c",
                    Module = "Intent.Security.JWT",
                    Title = "JWT Security Settings"
                };

                app.ModuleSettingGroups.Add(group);
            }

            var bearerAuthenticationType = group.Settings.FirstOrDefault(s => s.Id == "c2fba79a-6285-4ec8-ad76-585f089d8aa5");

            if (bearerAuthenticationType == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "c2fba79a-6285-4ec8-ad76-585f089d8aa5",
                    Title = "JWT Bearer Authentication Type",
                    Module = "Intent.Security.JWT",
                    Hint = "Configure how you JWT Token is authenticated",
                    ControlType = ModuleSettingControlType.Select,
                    Value = "manual",
                    Options =
                    [
                        new SettingFieldOptionPersistable { Description = "OIDC JWT Auth", Value = "oidc" },
                        new SettingFieldOptionPersistable { Description = "Manual JWT Auth", Value = "manual" }
                    ]
                });
            }
            else
            {
                bearerAuthenticationType.Value = "manual";
            }

            app.SaveAllChanges();
        }
    }
}