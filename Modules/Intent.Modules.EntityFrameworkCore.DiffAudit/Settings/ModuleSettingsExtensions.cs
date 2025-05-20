using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.EntityFrameworkCore.DiffAudit.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DiffAudit GetDiffAudit(this IApplicationSettingsProvider settings)
        {
            return new DiffAudit(settings.GetGroup("af5069a0-967a-4ffd-95b1-8bf6d305375c"));
        }
    }

    public class DiffAudit : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DiffAudit(IGroupSettings groupSettings)
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
        public UserIdentityToAuditOptions UserIdentityToAudit() => new UserIdentityToAuditOptions(_groupSettings.GetSetting("bf94aeeb-5e26-485e-9f1a-79bcb1ac158b")?.Value);

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