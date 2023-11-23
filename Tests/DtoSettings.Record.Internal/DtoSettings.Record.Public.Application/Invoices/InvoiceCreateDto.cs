using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record InvoiceCreateDto
    {
        public InvoiceCreateDto(string number, List<InvoiceLineCreateDto> invoiceLines)
        {
            Number = number;
            InvoiceLines = invoiceLines;
        }

        protected InvoiceCreateDto()
        {
            Number = null!;
            InvoiceLines = null!;
        }

        public string Number { get; internal set; }
        public List<InvoiceLineCreateDto> InvoiceLines { get; internal set; }
    }
}