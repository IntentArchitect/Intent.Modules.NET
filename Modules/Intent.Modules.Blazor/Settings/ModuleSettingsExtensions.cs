using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Blazor.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static Blazor GetBlazor(this IApplicationSettingsProvider settings)
        {
            return new Blazor(settings.GetGroup("489a67db-31b2-4d51-96d7-52637c3795be"));
        }
    }

    public class Blazor : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public Blazor(IGroupSettings groupSettings)
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

        public bool UseCodeBehindFilesForComponents() => bool.TryParse(_groupSettings.GetSetting("4c4c903b-6fbb-4a6b-b1f7-98a180f010f2")?.Value.ToPascalCase(), out var result) && result;
        public RenderModeOptions RenderMode() => new RenderModeOptions(_groupSettings.GetSetting("3e3d24f8-ad29-44d6-b7e5-e76a5af2a7fa")?.Value);

        public class RenderModeOptions
        {
            public readonly string Value;

            public RenderModeOptions(string value)
            {
                Value = value;
            }

            public RenderModeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "interactive-server" => RenderModeOptionsEnum.InteractiveServer,
                    "interactive-web-assembly" => RenderModeOptionsEnum.InteractiveWebAssembly,
                    "interactive-auto" => RenderModeOptionsEnum.InteractiveAuto,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInteractiveServer()
            {
                return Value == "interactive-server";
            }

            public bool IsInteractiveWebAssembly()
            {
                return Value == "interactive-web-assembly";
            }

            public bool IsInteractiveAuto()
            {
                return Value == "interactive-auto";
            }
        }

        public enum RenderModeOptionsEnum
        {
            InteractiveServer,
            InteractiveWebAssembly,
            InteractiveAuto,
        }
    }
}