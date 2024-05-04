using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Publish.CleanArchDapr.TestApplication.Domain.Common.Exceptions;
using Publish.CleanArchDapr.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Publish.CleanArchDapr.TestApplication.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Ignore)]
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingCustomer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            existingCustomer.Name = request.Name;

        }
    }
}