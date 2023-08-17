using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.Services.Invoices
{
    public class InvoiceUpdateDto
    {
        public InvoiceUpdateDto()
        {
            Number = null!;
        }
        public static InvoiceUpdateDto Create(Guid id, string number)
        {
            return new InvoiceUpdateDto
            {
                Id = id,
                Number = number
            };
        }

        public Guid Id { get; set; }

        public string Number { get; set; }
    }
}