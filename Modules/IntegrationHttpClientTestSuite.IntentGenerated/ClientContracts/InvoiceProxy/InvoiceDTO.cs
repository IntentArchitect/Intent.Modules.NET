using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace IntegrationHttpClientTestSuite.IntentGenerated.ClientContracts.InvoiceProxy
{
    public class InvoiceDTO
    {
        public static InvoiceDTO Create(
            Guid id,
            string reference)
        {
            return new InvoiceDTO
            {
                Id = id,
                Reference = reference,
            };
        }
        public Guid Id { get; set; }
        public string Reference { get; set; }
    }
}