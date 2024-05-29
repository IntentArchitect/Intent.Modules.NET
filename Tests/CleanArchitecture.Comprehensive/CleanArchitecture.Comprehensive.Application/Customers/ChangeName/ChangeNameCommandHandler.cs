using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Comprehensive.Domain.Common.Exceptions;
using CleanArchitecture.Comprehensive.Domain.Repositories.DDD;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Customers.ChangeName
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ChangeNameCommandHandler : IRequestHandler<ChangeNameCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public ChangeNameCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(ChangeNameCommand request, CancellationToken cancellationToken)
        {
            var entity = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (entity is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            entity.ChangeName(request.Name, request.Surname);
        }
    }
}