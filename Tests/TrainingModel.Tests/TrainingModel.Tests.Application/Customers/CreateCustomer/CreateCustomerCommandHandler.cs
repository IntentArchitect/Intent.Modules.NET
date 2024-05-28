using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TrainingModel.Tests.Domain.Entities;
using TrainingModel.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace TrainingModel.Tests.Application.Customers.CreateCustomer
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
            var customer = new Customer
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
                IsActive = request.IsActive,
                Preferences = new Preferences
                {
                    Specials = request.Specials,
                    News = request.News
                },
                Address = request.Address
                    .Select(a => new Address
                    {
                        Line1 = a.Line1,
                        Line2 = a.Line2,
                        City = a.City,
                        Postal = a.Postal,
                        AddressType = a.AddressType
                    })
                    .ToList()
            };

            _customerRepository.Add(customer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return customer.Id;
        }
    }
}