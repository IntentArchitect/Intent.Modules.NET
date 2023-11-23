using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Internal.Application.Invoices
{
    public class InvoiceUpdateDto
    {
        public InvoiceUpdateDto(Guid id, string number, List<InvoiceLineDto> invoiceLines)
        {
            Id = id;
            Number = number;
            InvoiceLines = invoiceLines;
        }

        protected InvoiceUpdateDto()
        {
            Number = null!;
            InvoiceLines = null!;
        }

        public Guid Id { get; internal set; }
        public string Number { get; internal set; }
        public List<InvoiceLineDto> InvoiceLines { get; internal set; }
    }
}