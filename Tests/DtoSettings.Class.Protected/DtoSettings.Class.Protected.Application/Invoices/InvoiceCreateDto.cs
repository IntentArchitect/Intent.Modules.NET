using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Protected.Application.Invoices
{
    public class InvoiceCreateDto
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

        public string Number { get; protected set; }
        public List<InvoiceLineCreateDto> InvoiceLines { get; protected set; }

        public static InvoiceCreateDto Create(string number, List<InvoiceLineCreateDto> invoiceLines)
        {
            return new InvoiceCreateDto(number, invoiceLines);
        }
    }
}