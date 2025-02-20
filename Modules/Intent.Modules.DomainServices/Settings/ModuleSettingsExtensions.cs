using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Modelers.Domain.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.DomainServices.Settings
{

    public static class DomainSettingsExtensions
    {
        public static DefaultDomainServiceScopeOptions DefaultDomainServiceScope(this DomainSettings groupSettings) => new DefaultDomainServiceScopeOptions(groupSettings.GetSetting("03248aef-bdb6-4ec6-8b83-c7e8af6310c6")?.Value);

        public class DefaultDomainServiceScopeOptions
        {
            public readonly string Value;

            public DefaultDomainServiceScopeOptions(string value)
            {
                Value = value;
            }

            public DefaultDomainServiceScopeOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "Transient" => DefaultDomainServiceScopeOptionsEnum.Transient,
                    "Scoped" => DefaultDomainServiceScopeOptionsEnum.Scoped,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsTransient()
            {
                return Value == "Transient";
            }

            public bool IsScoped()
            {
                return Value == "Scoped";
            }
        }

        public enum DefaultDomainServiceScopeOptionsEnum
        {
            Transient,
            Scoped,
        }
    }
}