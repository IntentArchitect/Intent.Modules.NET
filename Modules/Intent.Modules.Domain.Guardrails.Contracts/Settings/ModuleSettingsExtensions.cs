using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Domain.Guardrails.Contracts.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static DomainGuardrailsContractsSettings GetDomainGuardrailsContractsSettings(this IApplicationSettingsProvider settings)
        {
            return new DomainGuardrailsContractsSettings(settings.GetGroup("cfc23486-153d-4d9d-9e0b-cfeb1754e382"));
        }
    }

    public class DomainGuardrailsContractsSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public DomainGuardrailsContractsSettings(IGroupSettings groupSettings)
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
        public DefaultTypeModifiersOptions DefaultTypeModifiers() => new DefaultTypeModifiersOptions(_groupSettings.GetSetting("4bfe1393-e432-4171-870a-3f662cc61898")?.Value);

        public class DefaultTypeModifiersOptions
        {
            public readonly string Value;

            public DefaultTypeModifiersOptions(string value)
            {
                Value = value;
            }

            public DefaultTypeModifiersOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "public" => DefaultTypeModifiersOptionsEnum.Public,
                    "internal" => DefaultTypeModifiersOptionsEnum.Internal,
                    "public sealed" => DefaultTypeModifiersOptionsEnum.PublicSealed,
                    "internal sealed" => DefaultTypeModifiersOptionsEnum.InternalSealed,
                    "public partial" => DefaultTypeModifiersOptionsEnum.PublicPartial,
                    "internal partial" => DefaultTypeModifiersOptionsEnum.InternalPartial,
                    "public sealed partial" => DefaultTypeModifiersOptionsEnum.PublicSealedPartial,
                    "internal sealed partial" => DefaultTypeModifiersOptionsEnum.InternalSealedPartial,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsPublic()
            {
                return Value == "public";
            }

            public bool IsInternal()
            {
                return Value == "internal";
            }

            public bool IsPublicSealed()
            {
                return Value == "public sealed";
            }

            public bool IsInternalSealed()
            {
                return Value == "internal sealed";
            }

            public bool IsPublicPartial()
            {
                return Value == "public partial";
            }

            public bool IsInternalPartial()
            {
                return Value == "internal partial";
            }

            public bool IsPublicSealedPartial()
            {
                return Value == "public sealed partial";
            }

            public bool IsInternalSealedPartial()
            {
                return Value == "internal sealed partial";
            }
        }

        public enum DefaultTypeModifiersOptionsEnum
        {
            Public,
            Internal,
            PublicSealed,
            InternalSealed,
            PublicPartial,
            InternalPartial,
            PublicSealedPartial,
            InternalSealedPartial,
        }
    }
}