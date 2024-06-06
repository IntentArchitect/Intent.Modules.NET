using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects
{
    public class UpdateSubmissionItemDto
    {
        public UpdateSubmissionItemDto()
        {
            Key = null!;
            Value = null!;
        }

        public string Key { get; set; }
        public string Value { get; set; }
        public Guid Id { get; set; }

        public static UpdateSubmissionItemDto Create(string key, string value, Guid id)
        {
            return new UpdateSubmissionItemDto
            {
                Key = key,
                Value = value,
                Id = id
            };
        }
    }
}