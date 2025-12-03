using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Application.Dtos.Pagination.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static PaginationSettings GetPaginationSettings(this IApplicationSettingsProvider settings)
        {
            return new PaginationSettings(settings.GetGroup("f095c190-7906-4618-b2e6-fb8fe26708e6"));
        }
    }

    public class PaginationSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public PaginationSettings(IGroupSettings groupSettings)
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

        public string PageSizeDefault() => _groupSettings.GetSetting("b406f718-bd47-4e5d-890f-b96b7c71ad2e")?.Value;

        public string OrderByDefault() => _groupSettings.GetSetting("0556e2b8-9fc5-4376-ae15-978cc5c18f3f")?.Value;
    }
}