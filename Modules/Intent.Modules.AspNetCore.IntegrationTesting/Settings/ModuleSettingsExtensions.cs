using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.IntegrationTesting.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static IntegrationTestSettings GetIntegrationTestSettings(this IApplicationSettingsProvider settings)
        {
            return new IntegrationTestSettings(settings.GetGroup("d37f669a-8f2c-49ed-8c28-6ad31d836754"));
        }
    }

    public class IntegrationTestSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public IntegrationTestSettings(IGroupSettings groupSettings)
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
        public ContainerIsolationOptions ContainerIsolation() => new ContainerIsolationOptions(_groupSettings.GetSetting("e3ac09d9-db67-4432-8c77-44d38fe228a9")?.Value);

        public class ContainerIsolationOptions
        {
            public readonly string Value;

            public ContainerIsolationOptions(string value)
            {
                Value = value;
            }

            public ContainerIsolationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "container-per-test-class" => ContainerIsolationOptionsEnum.ContainerPerTestClass,
                    "shared-container" => ContainerIsolationOptionsEnum.SharedContainer,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsContainerPerTestClass()
            {
                return Value == "container-per-test-class";
            }

            public bool IsSharedContainer()
            {
                return Value == "shared-container";
            }
        }

        public enum ContainerIsolationOptionsEnum
        {
            ContainerPerTestClass,
            SharedContainer,
        }

        public bool GenerateServiceProxiesForTesting() => bool.TryParse(_groupSettings.GetSetting("1ba26513-e4ac-41f2-bba6-311b2cc2153b")?.Value.ToPascalCase(), out var result) && result;
        public IntegrationTestGenerationModeOptions IntegrationTestGenerationMode() => new IntegrationTestGenerationModeOptions(_groupSettings.GetSetting("1c5339c8-1f86-4058-af02-9fe200738fa3")?.Value);

        public class IntegrationTestGenerationModeOptions
        {
            public readonly string Value;

            public IntegrationTestGenerationModeOptions(string value)
            {
                Value = value;
            }

            public IntegrationTestGenerationModeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "all" => IntegrationTestGenerationModeOptionsEnum.All,
                    "explicit" => IntegrationTestGenerationModeOptionsEnum.Explicit,
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

        public enum IntegrationTestGenerationModeOptionsEnum
        {
            All,
            Explicit,
        }
    }
}