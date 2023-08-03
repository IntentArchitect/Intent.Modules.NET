using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Entities.PrivateSetters.TestApplication.Application.Invoices
{
    public class OperationLineDataContractDto
    {
        public OperationLineDataContractDto()
        {
            Description = null!;
        }

        public string Description { get; set; }
        public int Quantity { get; set; }

        public static OperationLineDataContractDto Create(string description, int quantity)
        {
            return new OperationLineDataContractDto
            {
                Description = description,
                Quantity = quantity
            };
        }
    }
}