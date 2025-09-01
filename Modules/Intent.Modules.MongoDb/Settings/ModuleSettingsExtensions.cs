using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.MongoDb.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static MongoDBSettings GetMongoDBSettings(this IApplicationSettingsProvider settings)
        {
            return new MongoDBSettings(settings.GetGroup("65b66781-0c91-48b4-990e-b9456f203ca6"));
        }
    }

    public class MongoDBSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public MongoDBSettings(IGroupSettings groupSettings)
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

        public bool PersistPrimaryKeyAsObjectId() => bool.TryParse(_groupSettings.GetSetting("5a17d713-b7f7-45c2-ab23-3614e2f95509")?.Value.ToPascalCase(), out var result) && result;

        public bool AlwaysIncludeDiscriminatorInDocuments() => bool.TryParse(_groupSettings.GetSetting("24bcf37a-a0fe-49f3-8121-bdad543c4fe6")?.Value.ToPascalCase(), out var result) && result;
    }

    public static class MultitenancySettingsExtensions
    {

        public static MongoDbDataIsolationOptions MongoDbDataIsolation(this MultitenancySettings groupSettings) => new MongoDbDataIsolationOptions(groupSettings.GetSetting("135d48e9-87cb-4380-8a3b-82e4f7553f58")?.Value);

        public class MongoDbDataIsolationOptions
        {
            public readonly string Value;

            public MongoDbDataIsolationOptions(string value)
            {
                Value = value;
            }

            public MongoDbDataIsolationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "none" => MongoDbDataIsolationOptionsEnum.None,
                    "separate-database" => MongoDbDataIsolationOptionsEnum.SeparateDatabase,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsSeparateDatabase()
            {
                return Value == "separate-database";
            }
        }

        public enum MongoDbDataIsolationOptionsEnum
        {
            None,
            SeparateDatabase,
        }
    }
}