using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Consolidated.Domain.Common.Exceptions;
using TestMediator.Consolidated.Domain.Repositories.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace TestMediator.Consolidated.Application.Customers
{
    public class GetCustomerByIdQuery : IQuery<CustomerDto>
    {
        public GetCustomerByIdQuery(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDto>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<CustomerDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null)
            {
                throw new NotFoundException($"Could not find Customer '{request.Id}'");
            }

            return new CustomerDto { Id = customer.Id, Name = customer.Name, Email = customer.Email };
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomerByIdQueryValidator : AbstractValidator<GetCustomerByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomerByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Merge)]
        private void ConfigureValidationRules()
        {
        // Implement custom validation logic here if required
        }
    }
}
