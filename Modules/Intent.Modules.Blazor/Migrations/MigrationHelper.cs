using Intent.IArchitect.Agent.Persistence.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Blazor.Migrations
{
    internal static class MigrationHelper
    {
        internal static bool InitializeIncludeSamplesSetting(ApplicationPersistable app, string value)
        {
            var group = app.ModuleSettingGroups.FirstOrDefault(x => x.Id == "489a67db-31b2-4d51-96d7-52637c3795be");//Blazor
            if (group == null)
            {
                group = new ApplicationModuleSettingsPersistable
                {
                    Id = "489a67db-31b2-4d51-96d7-52637c3795be",
                    Module = "Intent.Blazor",
                    Title = "Blazor Settings",
                    Settings = []
                };

                app.ModuleSettingGroups.Add(group);
            }

            var includeSamplesSetting = group.Settings.FirstOrDefault(s => s.Id == "bbddad39-3601-4244-bf8d-0da805ee6376");//Include Sample Pages

            if (includeSamplesSetting == null)
            {
                group.Settings.Add(new ModuleSettingPersistable
                {
                    Id = "bbddad39-3601-4244-bf8d-0da805ee6376",
                    Title = "Include Sample Pages",
                    Module = "Intent.Blazor",
                    ControlType = ModuleSettingControlType.Switch,
                    Value = value,
                });
                return true;
            }
            return false;
        }
    }
}
