using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace Standard.AspNetCore.TestApplication.Application.IntegrationServices.Services.Invoices
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