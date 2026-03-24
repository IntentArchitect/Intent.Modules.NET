using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record DocumentTitleDto
    {
        public DocumentTitleDto()
        {
            Value = null!;
        }

        public string Value { get; init; }
    }
}