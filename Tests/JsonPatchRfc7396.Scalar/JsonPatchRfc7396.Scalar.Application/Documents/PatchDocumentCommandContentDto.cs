using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record PatchDocumentCommandContentDto
    {
        public PatchDocumentCommandContentDto()
        {
            Format = null!;
            Text = null!;
            Json = null!;
        }

        public string Format { get; set; }
        public string Text { get; set; }
        public string Json { get; set; }
    }
}