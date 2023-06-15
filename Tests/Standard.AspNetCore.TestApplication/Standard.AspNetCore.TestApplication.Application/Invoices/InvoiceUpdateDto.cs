using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Invoices
{
    public class InvoiceUpdateDto
    {
        public InvoiceUpdateDto()
        {
            Number = null!;
        }

        public Guid Id { get; set; }
        public string Number { get; set; }

        public static InvoiceUpdateDto Create(Guid id, string number)
        {
            return new InvoiceUpdateDto
            {
                Id = id,
                Number = number
            };
        }
    }
}