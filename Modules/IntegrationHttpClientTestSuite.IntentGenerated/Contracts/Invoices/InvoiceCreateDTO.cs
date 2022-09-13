using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.Contracts.Invoices
{
    public class InvoiceCreateDTO
    {
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