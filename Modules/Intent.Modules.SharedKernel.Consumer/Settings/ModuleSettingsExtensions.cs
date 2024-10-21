using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.SharedKernel.Consumer.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static SharedKernel GetSharedKernel(this IApplicationSettingsProvider settings)
        {
            return new SharedKernel(settings.GetGroup("7bc19467-be53-4cf0-b910-a129dcbd5f69"));
        }
    }

    public class SharedKernel : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public SharedKernel(IGroupSettings groupSettings)
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

        public string SharedKernelApplicationId() => _groupSettings.GetSetting("f2f266ae-4717-4dc6-9f53-c7e872525b21")?.Value;
    }
}