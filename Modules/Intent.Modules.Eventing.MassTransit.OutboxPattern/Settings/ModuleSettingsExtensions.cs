using System;
using Intent.Configuration;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.Modules.Eventing.MassTransit.Settings;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions", Version = "1.0")]

namespace Intent.Modules.Eventing.MassTransit.OutboxPattern.Settings
{

    public static class EventingSettingsExtensions
    {
        public static OutboxPersistenceOptions OutboxPersistence(this EventingSettings groupSettings) => new OutboxPersistenceOptions(groupSettings.GetSetting("8acab111-2a3a-430c-b4f7-18eabe5e4045")?.Value);

        public class OutboxPersistenceOptions
        {
            public readonly string Value;

            public OutboxPersistenceOptions(string value)
            {
                Value = value;
            }

            public OutboxPersistenceOptionsEnum AsEnum()
            {
                return Value switch
                {
                    "in-memory" => OutboxPersistenceOptionsEnum.InMemory,
                    "entity-framework" => OutboxPersistenceOptionsEnum.EntityFramework,
                    _ => throw new ArgumentOutOfRangeException(nameof(Value), $"{Value} is out of range")
                };
            }

            public bool IsInMemory()
            {
                return Value == "in-memory";
            }

            public bool IsEntityFramework()
            {
                return Value == "entity-framework";
            }
        }

        public enum OutboxPersistenceOptionsEnum
        {
            InMemory,
            EntityFramework,
        }
    }
}