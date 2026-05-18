using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Orders
{
    public class CreateOrderCommandDispatchDocumentDto
    {
        public CreateOrderCommandDispatchDocumentDto()
        {
            DocumentNumber = null!;
        }

        public string DocumentNumber { get; set; }
        public DateTime IssuedOn { get; set; }
        public string? FileUrl { get; set; }

        public static CreateOrderCommandDispatchDocumentDto Create(string documentNumber, DateTime issuedOn, string? fileUrl)
        {
            return new CreateOrderCommandDispatchDocumentDto
            {
                DocumentNumber = documentNumber,
                IssuedOn = issuedOn,
                FileUrl = fileUrl
            };
        }
    }
}