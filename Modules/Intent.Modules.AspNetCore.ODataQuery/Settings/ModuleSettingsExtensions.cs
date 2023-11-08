using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.ODataQuery.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ODataQuerySettings GetODataQuerySettings(this IApplicationSettingsProvider settings)
        {
            return new ODataQuerySettings(settings.GetGroup("f9a82d4b-bb1b-4fa5-9586-36485364e6fc"));
        }
    }

    public class ODataQuerySettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ODataQuerySettings(IGroupSettings groupSettings)
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

        public bool AllowFilterOption() => bool.TryParse(_groupSettings.GetSetting("f07b3386-b22c-48b7-b39a-3b113d0984e3")?.Value.ToPascalCase(), out var result) && result;

        public bool AllowOrderByOption() => bool.TryParse(_groupSettings.GetSetting("35ddc434-96db-4d85-b8e8-8b724e251897")?.Value.ToPascalCase(), out var result) && result;

        public bool AllowExpandOption() => bool.TryParse(_groupSettings.GetSetting("e1ef66ba-3e3c-47c5-8a25-55bd827fcd3b")?.Value.ToPascalCase(), out var result) && result;

        public bool AllowSelectOption() => bool.TryParse(_groupSettings.GetSetting("73866e5a-1fba-4637-af2b-9c3cfc1786cd")?.Value.ToPascalCase(), out var result) && result;

        public string MaxTop() => _groupSettings.GetSetting("8bafbb9e-e756-4e07-8ff3-da57e4b3c190")?.Value;
    }
}