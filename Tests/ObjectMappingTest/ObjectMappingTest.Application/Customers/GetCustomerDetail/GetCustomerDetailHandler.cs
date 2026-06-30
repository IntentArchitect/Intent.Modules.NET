using Intent.RoslynWeaver.Attributes;
using MediatR;
using ObjectMappingTest.Domain.Common.Exceptions;
using ObjectMappingTest.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace ObjectMappingTest.Application.Customers.GetCustomerDetail
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerDetailHandler : IRequestHandler<GetCustomerDetail, CustomerDetailDto>
    {
        private readonly ICustomerRepository _customerRepository;

        [IntentManaged(Mode.Merge)]
        public GetCustomerDetailHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task<CustomerDetailDto> Handle(GetCustomerDetail request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customer is null) throw new NotFoundException($"Could not find Customer '{request.Id}'");
            return customer.MapToCustomerDetailDto();
        }
    }
}