using Intent.Configuration;
using Intent.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.MongoDb.Settings
{
    public class MultitenancySettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public MultitenancySettings(IGroupSettings groupSettings)
        {
            _groupSettings = groupSettings;
        }

        public string Id => _groupSettings.Id;

        public string Title
        {
            get => _groupSettings.Title;
            set => _groupSettings.Title = value;
        }

        public ISetting GetSetting(string settingId)
        {
            return _groupSettings.GetSetting(settingId);
        }
    }

    internal static class ModuleSettingsExtensions
    {
        public static MultitenancySettings GetMultitenancySettings(this IApplicationSettingsProvider settings)
        {
            return new MultitenancySettings(settings.GetGroup("41ae5a02-3eb2-42a6-ade2-322b3c1f1115"));
        }
    }

}
