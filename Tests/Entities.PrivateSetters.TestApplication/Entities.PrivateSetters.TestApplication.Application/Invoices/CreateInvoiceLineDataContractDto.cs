using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices
{
    public class CreateInvoiceLineDataContractDto
    {
        public CreateInvoiceLineDataContractDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public int Quantity { get; set; }

        public static CreateInvoiceLineDataContractDto Create(string description, int quantity)
        {
            return new CreateInvoiceLineDataContractDto
            {
                Description = description,
                Quantity = quantity
            };
        }
    }
}