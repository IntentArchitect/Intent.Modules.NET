using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Entities.BasicAuditing.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static BasicAuditing GetBasicAuditing(this IApplicationSettingsProvider settings)
        {
            return new BasicAuditing(settings.GetGroup("e51c0868-816d-432b-9cc3-c597fdb1ef0d"));
        }
    }

    public class BasicAuditing : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public BasicAuditing(IGroupSettings groupSettings)
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
        public UserIdentityToAuditOptions UserIdentityToAudit() => new UserIdentityToAuditOptions(_groupSettings.GetSetting("5f617e3b-b027-4b23-aeb5-5ee4c7968173")?.Value);

        public class UserIdentityToAuditOptions
        {
            public readonly string Value;

            public UserIdentityToAuditOptions(string value)
            {
                Value = value;
            }

            public UserIdentityToAuditOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "user-id" => UserIdentityToAuditOptionsEnum.UserId,
                    "user-name" => UserIdentityToAuditOptionsEnum.UserName,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsUserId()
            {
                return Value == "user-id";
            }

            public bool IsUserName()
            {
                return Value == "user-name";
            }
        }

        public enum UserIdentityToAuditOptionsEnum
        {
            UserId,
            UserName,
        }
        public ApplyBasicAuditingToEntitiesOptions ApplyBasicAuditingToEntities() => new ApplyBasicAuditingToEntitiesOptions(_groupSettings.GetSetting("43a36ffb-2085-4cf5-a255-f95c19368fa8")?.Value);

        public class ApplyBasicAuditingToEntitiesOptions
        {
            public readonly string Value;

            public ApplyBasicAuditingToEntitiesOptions(string value)
            {
                Value = value;
            }

            public ApplyBasicAuditingToEntitiesOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "manually" => ApplyBasicAuditingToEntitiesOptionsEnum.Manually,
                    "automatically-when-created" => ApplyBasicAuditingToEntitiesOptionsEnum.AutomaticallyWhenCreated,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsManually()
            {
                return Value == "manually";
            }

            public bool IsAutomaticallyWhenCreated()
            {
                return Value == "automatically-when-created";
            }
        }

        public enum ApplyBasicAuditingToEntitiesOptionsEnum
        {
            Manually,
            AutomaticallyWhenCreated,
        }

        public string CreatedByFieldName() => _groupSettings.GetSetting("739d748f-1e78-4588-8ff8-375b0438ec5e")?.Value;

        public string CreatedDateFieldName() => _groupSettings.GetSetting("16109f2c-5ce0-43fc-aded-a1ca2bde6525")?.Value;

        public string UpdatedByFieldName() => _groupSettings.GetSetting("2b583725-0e06-42f8-824c-f1922e8af1ba")?.Value;

        public string UpdatedDateFieldName() => _groupSettings.GetSetting("60c8ed40-0b0d-4bdf-8e08-56c126835438")?.Value;

        public bool IncludeCreatedByField() => bool.TryParse(_groupSettings.GetSetting("16565573-5078-41c7-b083-e68b51154782")?.Value.ToPascalCase(), out var result) && result;

        public bool IncludeCreatedDateField() => bool.TryParse(_groupSettings.GetSetting("fb3a6f03-017f-499d-b44b-cb519b653ca6")?.Value.ToPascalCase(), out var result) && result;

        public bool IncludeUpdatedByField() => bool.TryParse(_groupSettings.GetSetting("5e86a4ca-df1d-4f63-a1db-c90a4fb70d24")?.Value.ToPascalCase(), out var result) && result;

        public bool IncludeUpdatedDateField() => bool.TryParse(_groupSettings.GetSetting("f936421d-7f37-4205-a98b-deea631a9291")?.Value.ToPascalCase(), out var result) && result;
    }
}