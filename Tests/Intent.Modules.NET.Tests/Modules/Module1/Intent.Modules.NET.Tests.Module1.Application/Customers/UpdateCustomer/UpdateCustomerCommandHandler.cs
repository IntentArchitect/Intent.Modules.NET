using Intent.Modules.NET.Tests.Domain.Core.Common.Exceptions;
using Intent.Modules.NET.Tests.Module1.Application.Contracts.Customers.UpdateCustomer;
using Intent.Modules.NET.Tests.Module1.Domain.Common.Interfaces;
using Intent.Modules.NET.Tests.Module1.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Intent.Modules.NET.Tests.Module1.Application.Customers.UpdateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;

        [IntentManaged(Mode.Merge)]
        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            customer.Name = request.Name;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}