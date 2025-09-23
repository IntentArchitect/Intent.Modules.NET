using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.Customers.DeleteAddress
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteAddressCommandHandler : IRequestHandler<DeleteAddressCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteAddressCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.CustomerId, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.CustomerId}'");
            }

            var address = customer.Addresses.FirstOrDefault(x => x.Id == request.Id);
            if (address is null)
            {
                throw new NotFoundException($"Could not find Address '{request.Id}'");
            }


            customer.Addresses.Remove(address);
        }
    }
}