using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Invoices
{
    public class InvoiceCreateDto
    {
        public InvoiceCreateDto()
        {
            Number = null!;
        }

        public string Number { get; set; }

        public static InvoiceCreateDto Create(string number)
        {
            return new InvoiceCreateDto
            {
                Number = number
            };
        }
    }
}