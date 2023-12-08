using AdvancedMappingCrud.Cosmos.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<string>, ICommand
    {
        public CreateCustomerCommand(string name, string surname, bool isActive)
        {
            Name = name;
            Surname = surname;
            IsActive = isActive;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public bool IsActive { get; set; }
    }
}