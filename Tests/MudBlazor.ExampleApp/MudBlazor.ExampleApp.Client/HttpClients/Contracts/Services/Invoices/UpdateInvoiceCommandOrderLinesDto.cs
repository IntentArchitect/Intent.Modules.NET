using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices
{
    public class UpdateInvoiceCommandOrderLinesDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }

        public static UpdateInvoiceCommandOrderLinesDto Create(
            Guid id,
            Guid productId,
            int units,
            decimal unitPrice,
            decimal? discount)
        {
            return new UpdateInvoiceCommandOrderLinesDto
            {
                Id = id,
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount
            };
        }
    }
}