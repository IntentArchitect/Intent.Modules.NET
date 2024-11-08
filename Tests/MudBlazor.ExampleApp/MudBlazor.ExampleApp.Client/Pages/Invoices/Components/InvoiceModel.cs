using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Client.ModelDefinitionTemplate", Version = "1.0")]

namespace MudBlazor.ExampleApp.Client.Pages.Invoices.Components
{
    public class InvoiceModel
    {
        public Guid Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public List<InvoiceLineModel> OrderLines { get; set; } = [];
        public Guid? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string? CustomerAccountNo { get; set; }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string AddressCity { get; set; }
        public string AddressCountry { get; set; }
        public string AddressPostal { get; set; }
        public DateTime? DueDate { get; set; }
        public string? Reference { get; set; }
    }
}