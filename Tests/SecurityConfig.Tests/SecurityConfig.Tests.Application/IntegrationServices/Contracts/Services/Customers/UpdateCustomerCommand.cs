using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Application.Contracts.Clients.DtoContract", Version = "2.0")]

namespace SecurityConfig.Tests.Application.IntegrationServices.Contracts.Services.Customers
{
    public class UpdateCustomerCommand
    {
        public UpdateCustomerCommand()
        {
            Name = null!;
            Surname = null!;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Id { get; set; }

        public static UpdateCustomerCommand Create(string name, string surname, Guid id)
        {
            return new UpdateCustomerCommand
            {
                Name = name,
                Surname = surname,
                Id = id
            };
        }
    }
}