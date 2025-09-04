using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.AspNetCore.Scalar.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ScalarSettings GetScalarSettings(this IApplicationSettingsProvider settings)
        {
            return new ScalarSettings(settings.GetGroup("8222cbb9-cf09-4869-a41f-1ce923b7379e"));
        }
    }

    public class ScalarSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ScalarSettings(IGroupSettings groupSettings)
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
        public AuthenticationOptions Authentication() => new AuthenticationOptions(_groupSettings.GetSetting("d845a606-c05b-4a11-9212-c5f099d890c6")?.Value);

        public class AuthenticationOptions
        {
            public readonly string Value;

            public AuthenticationOptions(string value)
            {
                Value = value;
            }

            public AuthenticationOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "Bearer" => AuthenticationOptionsEnum.Bearer,
                    "Implicit" => AuthenticationOptionsEnum.Implicit,
                    "None" => AuthenticationOptionsEnum.None,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsBearer()
            {
                return Value == "Bearer";
            }

            public bool IsImplicit()
            {
                return Value == "Implicit";
            }

            public bool IsNone()
            {
                return Value == "None";
            }
        }

        public enum AuthenticationOptionsEnum
        {
            Bearer,
            Implicit,
            None,
        }

        public bool UseFullyQualifiedSchemaIdentifiers() => bool.TryParse(_groupSettings.GetSetting("1bc3a9df-c63d-4a62-a734-5cf53cc3f629")?.Value.ToPascalCase(), out var result) && result;
    }
}