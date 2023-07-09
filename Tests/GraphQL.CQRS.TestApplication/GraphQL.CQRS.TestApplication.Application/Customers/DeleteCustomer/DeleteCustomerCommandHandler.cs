using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.CQRS.TestApplication.Domain.Common.Exceptions;
using GraphQL.CQRS.TestApplication.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace GraphQL.CQRS.TestApplication.Application.Customers.DeleteCustomer
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerDto> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var existingCustomer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);

            if (existingCustomer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}' ");
            }
            _customerRepository.Remove(existingCustomer);
            return existingCustomer.MapToCustomerDto(_mapper);
        }
    }
}