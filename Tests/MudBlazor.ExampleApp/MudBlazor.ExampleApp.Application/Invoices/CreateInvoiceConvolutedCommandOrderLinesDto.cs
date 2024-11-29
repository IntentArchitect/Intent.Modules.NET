using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace MudBlazor.ExampleApp.Application.Invoices
{
    public class CreateInvoiceConvolutedCommandOrderLinesDto
    {
        public CreateInvoiceConvolutedCommandOrderLinesDto()
        {
        }

        public Guid ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public static CreateInvoiceConvolutedCommandOrderLinesDto Create(
            Guid productId,
            int units,
            decimal unitPrice,
            decimal? discount)
        {
            return new CreateInvoiceConvolutedCommandOrderLinesDto
            {
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount
            };
        }
    }
}