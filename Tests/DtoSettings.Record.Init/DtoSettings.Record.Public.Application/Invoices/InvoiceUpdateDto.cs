using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace DtoSettings.Record.Public.Application.Invoices
{
    public record InvoiceUpdateDto
    {
        public InvoiceUpdateDto()
        {
            Number = null!;
            InvoiceLines = null!;
        }

        public Guid Id { get; init; }
        public string Number { get; init; }
        public List<InvoiceLineDto> InvoiceLines { get; init; }

        public static InvoiceUpdateDto Create(Guid id, string number, List<InvoiceLineDto> invoiceLines)
        {
            return new InvoiceUpdateDto
            {
                Id = id,
                Number = number,
                InvoiceLines = invoiceLines
            };
        }
    }
}