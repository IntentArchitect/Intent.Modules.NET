using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.ServiceContracts.Invoices
{

    public class InvoiceCreateDTO
    {
        public InvoiceCreateDTO()
        {
        }

        public static InvoiceCreateDTO Create(
            string reference)
        {
            return new InvoiceCreateDTO
            {
                Reference = reference,
            };
        }

        public string Reference { get; set; }

    }
}