using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.UnitTesting.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static UnitTestSettings GetUnitTestSettings(this IApplicationSettingsProvider settings)
        {
            return new UnitTestSettings(settings.GetGroup("d62269ea-8e64-44a0-8392-e1a69da7c960"));
        }
    }

    public class UnitTestSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public UnitTestSettings(IGroupSettings groupSettings)
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
        public UnitTestGenerationModeOptions UnitTestGenerationMode() => new UnitTestGenerationModeOptions(_groupSettings.GetSetting("381ddc17-ca81-4f28-8636-5a4b547ff6d4")?.Value);

        public class UnitTestGenerationModeOptions
        {
            public readonly string Value;

            public UnitTestGenerationModeOptions(string value)
            {
                Value = value;
            }

            public UnitTestGenerationModeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "all" => UnitTestGenerationModeOptionsEnum.All,
                    "explicit" => UnitTestGenerationModeOptionsEnum.Explicit,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsAll()
            {
                return Value == "all";
            }

            public bool IsExplicit()
            {
                return Value == "explicit";
            }
        }

        public enum UnitTestGenerationModeOptionsEnum
        {
            All,
            Explicit,
        }
        public MockFrameworkOptions MockFramework() => new MockFrameworkOptions(_groupSettings.GetSetting("115c28bc-a4c8-4b30-bd00-2e320fee77dc")?.Value);

        public class MockFrameworkOptions
        {
            public readonly string Value;

            public MockFrameworkOptions(string value)
            {
                Value = value;
            }

            public MockFrameworkOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "moq" => MockFrameworkOptionsEnum.Moq,
                    "nsubstitute" => MockFrameworkOptionsEnum.Nsubstitute,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsMoq()
            {
                return Value == "moq";
            }

            public bool IsNsubstitute()
            {
                return Value == "nsubstitute";
            }
        }

        public enum MockFrameworkOptionsEnum
        {
            Moq,
            Nsubstitute,
        }
    }
}