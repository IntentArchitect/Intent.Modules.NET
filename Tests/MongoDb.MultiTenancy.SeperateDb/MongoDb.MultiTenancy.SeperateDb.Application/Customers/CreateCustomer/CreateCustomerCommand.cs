using Intent.RoslynWeaver.Attributes;
using MediatR;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Customers.CreateCustomer
{
    public class CreateCustomerCommand : IRequest<string>, ICommand
    {
        public CreateCustomerCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}