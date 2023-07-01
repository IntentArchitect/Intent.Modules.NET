using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Contracts.DomainMapping.DtoExtensions", Version = "1.0")]

namespace MassTransit.Messages.Shared
{
    public static class PreferenceDtoExtensions
    {
        public static PreferenceDto MapToPreferenceDto(this Preference projectFrom)
        {
            return new PreferenceDto
            {
                Id = projectFrom.Id,
                Key = projectFrom.Key,
                Value = projectFrom.Value,
                UserId = projectFrom.UserId,
            };
        }
    }
}