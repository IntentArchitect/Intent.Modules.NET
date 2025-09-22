using AdvancedMapping.Repositories.Mapperly.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.DeleteAddress
{
    public class DeleteAddressCommand : IRequest, ICommand
    {
        public DeleteAddressCommand(Guid customerId, Guid id)
        {
            CustomerId = customerId;
            Id = id;
        }

        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
    }
}