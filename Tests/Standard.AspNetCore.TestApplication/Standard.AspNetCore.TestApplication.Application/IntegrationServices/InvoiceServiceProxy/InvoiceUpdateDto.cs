using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.InvoiceServiceProxy
{
    public class InvoiceUpdateDto
    {
        public static InvoiceUpdateDto Create(
            Guid id,
            string number)
        {
            return new InvoiceUpdateDto
            {
                Id = id,
                Number = number,
            };
        }

        public Guid Id { get; set; }

        public string Number { get; set; }
    }
}