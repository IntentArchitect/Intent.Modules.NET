using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace JsonPatchRfc7396.Scalar.Application.Documents
{
    public record UpdateDocumentTitleDto
    {
        public UpdateDocumentTitleDto()
        {
            Value = null!;
        }

        public string Value { get; set; }
    }
}