using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects
{
    public class CreateSubmissionItemDto
    {
        public CreateSubmissionItemDto()
        {
            Key = null!;
            Value = null!;
        }

        public string Key { get; set; }
        public string Value { get; set; }

        public static CreateSubmissionItemDto Create(string key, string value)
        {
            return new CreateSubmissionItemDto
            {
                Key = key,
                Value = value
            };
        }
    }
}