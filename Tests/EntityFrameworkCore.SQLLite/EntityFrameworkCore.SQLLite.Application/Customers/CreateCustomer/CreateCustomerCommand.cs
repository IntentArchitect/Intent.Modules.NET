using EntityFrameworkCore.SQLLite.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<Guid>, ICommand
    {
        public CreateCustomerCommand(string name, string suranme)
        {
            Name = name;
            Suranme = suranme;
        }

        public string Name { get; set; }
        public string Suranme { get; set; }
    }
}