using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Application.ServiceImplementations.Api
{
    internal static class ModuleSettingsExtensions
    {
        public static UnitOfWorkSettings? GetUnitOfWorkSettings(this IApplicationSettingsProvider settings)
        {
            var groupSetting = settings.GetGroup("c4b7e545-eaac-42bc-8f06-2768ac8dad99");
            if (groupSetting == null)
            {
                return null;
            }
            return new UnitOfWorkSettings(settings.GetGroup("c4b7e545-eaac-42bc-8f06-2768ac8dad99"));
        }
    }

    internal class UnitOfWorkSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public UnitOfWorkSettings(IGroupSettings groupSettings)
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

        public bool UseAmbientTransactions() => bool.TryParse(_groupSettings.GetSetting("f79a4c1d-4f4a-42b2-ad8e-81f974dfb0e2")?.Value.ToPascalCase(), out var result) && result;

        public bool AutomaticallyPersistUnitOfWork() => bool.TryParse(_groupSettings.GetSetting("d6338b7c-b0f9-46bd-8dbb-3c745d5f8623")?.Value.ToPascalCase(), out var result) && result;
    }
}
