using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.DtoContract", Version = "2.0")]

namespace MudBlazor.ExampleApp.Client.HttpClients.Contracts.Services.Invoices
{
    public class InvoiceInvoiceLineDto
    {
        public InvoiceInvoiceLineDto()
        {
            ProductName = null!;
            ProductDescription = null!;
        }

        public Guid InvoiceId { get; set; }
        public Guid? ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string? ProductImageUrl { get; set; }

        public static InvoiceInvoiceLineDto Create(
            Guid invoiceId,
            Guid? productId,
            int units,
            decimal unitPrice,
            decimal? discount,
            Guid id,
            string productName,
            string productDescription,
            string? productImageUrl)
        {
            return new InvoiceInvoiceLineDto
            {
                InvoiceId = invoiceId,
                ProductId = productId,
                Units = units,
                UnitPrice = unitPrice,
                Discount = discount,
                Id = id,
                ProductName = productName,
                ProductDescription = productDescription,
                ProductImageUrl = productImageUrl
            };
        }
    }
}