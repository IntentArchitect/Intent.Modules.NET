using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.DDD;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.Email = request.Email;
            customer.Name = request.Name;
            customer.Surname = request.Surname;
            customer.Address = new Address(
                line1: request.Address.Line1,
                line2: request.Address.Line2,
                city: request.Address.City,
                postal: request.Address.Postal);
        }
    }
}