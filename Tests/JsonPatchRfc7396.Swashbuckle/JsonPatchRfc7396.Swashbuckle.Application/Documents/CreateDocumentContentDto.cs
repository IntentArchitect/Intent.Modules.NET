using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Swashbuckle.Application.Documents
{
    public record CreateDocumentContentDto
    {
        public CreateDocumentContentDto()
        {
            Format = null!;
            Text = null!;
            Json = null!;
        }

        public string Format { get; init; }
        public string Text { get; init; }
        public object Json { get; init; }
    }
}