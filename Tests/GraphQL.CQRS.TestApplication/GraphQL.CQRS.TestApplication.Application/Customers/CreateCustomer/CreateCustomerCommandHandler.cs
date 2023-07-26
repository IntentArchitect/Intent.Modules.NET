using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Domain.Entities;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace GraphQL.CQRS.TestApplication.Application.Customers.CreateCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }


        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
        {
            var newCustomer = new Customer
            {
                Name = request.Name,
                Surname = request.Surname,
                Email = request.Email,
            };

            _customerRepository.Add(newCustomer);
            await _customerRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newCustomer.MapToCustomerDto(_mapper);
        }
    }
}