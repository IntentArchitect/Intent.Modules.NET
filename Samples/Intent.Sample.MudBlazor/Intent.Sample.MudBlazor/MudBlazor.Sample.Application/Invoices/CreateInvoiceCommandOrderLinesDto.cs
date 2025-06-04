using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.Sample.Application.Invoices
{
    public class CreateInvoiceCommandOrderLinesDto
    {
        public CreateInvoiceCommandOrderLinesDto()
        {
        }

        public Guid ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public static CreateInvoiceCommandOrderLinesDto Create(Guid productId, int units, decimal unitPrice, decimal? discount)
        {
            return new CreateInvoiceCommandOrderLinesDto
            {
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount
            };
        }
    }
}