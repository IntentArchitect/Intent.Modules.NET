using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DtoSettings.Class.Private.Domain.Entities;
using DtoSettings.Class.Private.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace DtoSettings.Class.Private.Application.Customers.CreateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public CreateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var newCustomer = new Customer
            {
                Name = request.Name,
                Surname = request.Surname,
                CreatedDate = request.CreatedDate,
            };

            _customerRepository.Add(newCustomer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCustomer.Id;
        }
    }
}