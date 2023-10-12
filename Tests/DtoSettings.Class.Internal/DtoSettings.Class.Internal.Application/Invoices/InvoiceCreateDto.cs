using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Invoices
{
    public class InvoiceCreateDto
    {
        public InvoiceCreateDto(string number, List<InvoiceLineCreateDto> invoiceLines)
        {
            Number = number;
            InvoiceLines = invoiceLines;
        }

        public string Number { get; internal set; }
        public List<InvoiceLineCreateDto> InvoiceLines { get; internal set; }

        // public static InvoiceCreateDto Create(string number, List<InvoiceLineCreateDto> invoiceLines)
        // {
        //     return new InvoiceCreateDto
        //     {
        //         Number = number,
        //         InvoiceLines = invoiceLines
        //     };
        // }
    }
}