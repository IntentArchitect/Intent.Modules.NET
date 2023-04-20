using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Integration.HttpClients.TestApplication.Application.Invoices
{

    public class InvoiceDTO
    {
        public InvoiceDTO()
        {
        }

        public static InvoiceDTO Create(Guid id, string reference)
        {
            return new InvoiceDTO
            {
                Id = id,
                Reference = reference
            };
        }

        public Guid Id { get; set; }

        public string Reference { get; set; }

    }
}