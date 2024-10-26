using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.ProjectTo.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.ProjectTo.Tests.Application.Customers.ChangeEmailCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeEmailCustomerCommandHandler : IRequestHandler<ChangeEmailCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public ChangeEmailCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeEmailCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.ChangeEmail(request.Email);
        }
    }
}