using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<string>, ICommand
    {
        public CreateCustomerCommand(string name, string surname, string email)
        {
            Name = name;
            Surname = surname;
            Email = email;
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
    }
}