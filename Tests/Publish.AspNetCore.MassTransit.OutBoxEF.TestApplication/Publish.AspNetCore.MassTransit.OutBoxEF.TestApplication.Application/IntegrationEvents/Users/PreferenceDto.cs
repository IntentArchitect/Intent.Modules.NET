using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.IntegrationEventDto", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public class PreferenceDto
    {
        public PreferenceDto()
        {
        }

        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public Guid UserId { get; set; }

        public static PreferenceDto Create(Guid id, string key, string value, Guid userId)
        {
            return new PreferenceDto
            {
                Id = id,
                Key = key,
                Value = value,
                UserId = userId
            };
        }
    }
}