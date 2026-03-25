using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record CreateDocumentTitleDto
    {
        public CreateDocumentTitleDto()
        {
            Value = null!;
        }

        public string Value { get; init; }
    }
}