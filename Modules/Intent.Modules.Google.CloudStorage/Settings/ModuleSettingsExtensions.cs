using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Google.CloudStorage.Settings
{

    public static class MultitenancySettingsExtensions
    {
        public static GoogleCloudStorageDataIsolationOptions GoogleCloudStorageDataIsolation(this MultitenancySettings groupSettings) => new GoogleCloudStorageDataIsolationOptions(groupSettings.GetSetting("47907e56-bda9-4ec8-945d-318b0fdc0322")?.Value);

        public class GoogleCloudStorageDataIsolationOptions
        {
            public readonly string Value;

            public GoogleCloudStorageDataIsolationOptions(string value)
            {
                Value = value;
            }

            public GoogleCloudStorageDataIsolationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => GoogleCloudStorageDataIsolationOptionsEnum.None,
                    "separate-storage-account" => GoogleCloudStorageDataIsolationOptionsEnum.SeparateStorageAccount,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsSeparateStorageAccount()
            {
                return Value == "separate-storage-account";
            }
        }

        public enum GoogleCloudStorageDataIsolationOptionsEnum
        {
            None,
            SeparateStorageAccount,
        }
    }
}