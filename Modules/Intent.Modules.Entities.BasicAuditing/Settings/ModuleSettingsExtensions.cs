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
    }
}