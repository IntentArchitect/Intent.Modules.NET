using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.ServiceContracts.Invoices
{

    public class InvoiceUpdateDTO
    {
        public InvoiceUpdateDTO()
        {
        }

        public static InvoiceUpdateDTO Create(
            string reference)
        {
            return new InvoiceUpdateDTO
            {
                Reference = reference,
            };
        }

        public string Reference { get; set; }

    }
}