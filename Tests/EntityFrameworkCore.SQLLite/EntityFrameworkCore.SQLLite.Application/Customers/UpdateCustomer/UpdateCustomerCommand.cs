using EntityFrameworkCore.SQLLite.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace EntityFrameworkCore.SQLLite.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(Guid id, string name, string suranme)
        {
            Id = id;
            Name = name;
            Suranme = suranme;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Suranme { get; set; }
    }
}