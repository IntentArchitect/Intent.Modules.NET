using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.PatchCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class PatchCustomerCommandHandler : IRequestHandler<PatchCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public PatchCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(PatchCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }


            if (request.Name is not null)
            {
                customer.Name = request.Name;
            }

            if (request.Surname is not null)
            {
                customer.Surname = request.Surname;
            }

            if (request.IsActive is not null)
            {
                customer.IsActive = request.IsActive.Value;
            }
            customer.Preferences ??= new Preferences();

            if (request.Newsletter is not null)
            {
                customer.Preferences.Newsletter = request.Newsletter.Value;
            }

            if (request.Specials is not null)
            {
                customer.Preferences.Specials = request.Specials.Value;
            }
        }
    }
}