using Intent.RoslynWeaver.Attributes;
using MediatR;
using MongoDb.MultiTenancy.SeperateDb.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Application.Customers.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest, ICommand
    {
        public UpdateCustomerCommand(string name, string id)
        {
            Name = name;
            Id = id;
        }

        public string Name { get; set; }
        public string Id { get; set; }
    }
}