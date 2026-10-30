using FluentValidation;
using Intent.RoslynWeaver.Attributes;
using Mediator;
using TestMediator.Consolidated.Domain.Repositories.Customers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Mediator.QueryModels", Version = "1.0")]

namespace TestMediator.Consolidated.Application.Customers
{
    public class GetCustomersQuery : IQuery<List<CustomerDto>>
    {
        public GetCustomersQuery()
        {
        }
    }

    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomersQueryHandler : IQueryHandler<GetCustomersQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async ValueTask<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.FindAllAsync(cancellationToken);
            return customers
                .Select(customer => new CustomerDto { Id = customer.Id, Name = customer.Name, Email = customer.Email })
                .ToList();
        }
    }

    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetCustomersQueryValidator : AbstractValidator<GetCustomersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetCustomersQueryValidator()
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
