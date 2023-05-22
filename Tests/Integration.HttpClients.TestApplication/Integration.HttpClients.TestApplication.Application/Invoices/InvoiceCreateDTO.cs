using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.Invoices
{

    public class InvoiceCreateDTO
    {
        public InvoiceCreateDTO()
        {
            Reference = null!;
        }

        public static InvoiceCreateDTO Create(string reference)
        {
            return new InvoiceCreateDTO
            {
                Reference = reference
            };
        }

        public string Reference { get; set; }

    }
}