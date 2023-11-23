using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Class.Private.Application.Invoices
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

        public Guid Id { get; private set; }
        public string Number { get; private set; }
        public List<InvoiceLineDto> InvoiceLines { get; private set; }

        public static InvoiceUpdateDto Create(Guid id, string number, List<InvoiceLineDto> invoiceLines)
        {
            return new InvoiceUpdateDto(id, number, invoiceLines);
        }
    }
}