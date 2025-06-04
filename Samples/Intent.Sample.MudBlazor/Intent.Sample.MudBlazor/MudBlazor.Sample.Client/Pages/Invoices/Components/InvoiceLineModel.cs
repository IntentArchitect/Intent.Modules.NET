using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.ModelDefinitionTemplate", Version = "1.0")]

namespace MudBlazor.Sample.Client.Pages.Invoices.Components
{
    public class InvoiceLineModel
    {
        public Guid InvoiceId { get; set; }
        public Guid? ProductId { get; set; }
        public int Units { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal? Discount { get; set; }
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}