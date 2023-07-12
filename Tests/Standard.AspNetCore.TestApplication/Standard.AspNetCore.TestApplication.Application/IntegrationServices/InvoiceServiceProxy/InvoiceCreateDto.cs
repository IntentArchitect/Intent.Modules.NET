using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.InvoiceServiceProxy
{
    public class InvoiceCreateDto
    {
        public static InvoiceCreateDto Create(string number)
        {
            return new InvoiceCreateDto
            {
                Number = number
            };
        }

        public string Number { get; set; }
    }
}