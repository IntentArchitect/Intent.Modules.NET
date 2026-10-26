using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Services.Guardrails.Contracts.Settings
{
    public static class ModuleSettingsExtensions
    {
        public static ServicesGuardrailsContractsSettings GetServicesGuardrailsContractsSettings(this IApplicationSettingsProvider settings)
        {
            return new ServicesGuardrailsContractsSettings(settings.GetGroup("c686f0ee-a3a9-4933-b7bc-9ba35b6530f3"));
        }
    }

    public class ServicesGuardrailsContractsSettings : IGroupSettings
    {
        private readonly IGroupSettings _groupSettings;

        public ServicesGuardrailsContractsSettings(IGroupSettings groupSettings)
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
        public DefaultTypeModifiersOptions DefaultTypeModifiers() => new DefaultTypeModifiersOptions(_groupSettings.GetSetting("d0c9c937-5c8f-4adb-be14-b9bdd3748741")?.Value);

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
                    "none" => DefaultTypeModifiersOptionsEnum.None,
                    "sealed" => DefaultTypeModifiersOptionsEnum.Sealed,
                    "partial" => DefaultTypeModifiersOptionsEnum.Partial,
                    "sealed partial" => DefaultTypeModifiersOptionsEnum.SealedPartial,
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

            public bool IsNone()
            {
                return Value == "none";
            }

            public bool IsSealed()
            {
                return Value == "sealed";
            }

            public bool IsPartial()
            {
                return Value == "partial";
            }

            public bool IsSealedPartial()
            {
                return Value == "sealed partial";
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
            None,
            Sealed,
            Partial,
            SealedPartial,
        }
    }
}