using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AzureFunctions.AppSettings.Application
{
    public record PayloadDto
    {
        public PayloadDto()
        {
            Id = null!;
            Value = null!;
        }

        public string Id { get; init; }
        public string Value { get; init; }
    }
}