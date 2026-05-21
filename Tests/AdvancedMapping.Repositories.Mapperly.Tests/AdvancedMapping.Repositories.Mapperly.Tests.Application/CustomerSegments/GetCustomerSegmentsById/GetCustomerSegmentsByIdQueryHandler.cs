using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Common.Exceptions;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegmentsById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerSegmentsByIdQueryHandler : IRequestHandler<GetCustomerSegmentsByIdQuery, CustomerSegmentsDto>
    {
        private readonly ICustomerSegmentsRepository _customerSegmentsRepository;
        private readonly CustomerSegmentsDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomerSegmentsByIdQueryHandler(ICustomerSegmentsRepository customerSegmentsRepository,
            CustomerSegmentsDtoMapper mapper)
        {
            _customerSegmentsRepository = customerSegmentsRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerSegmentsDto> Handle(
            GetCustomerSegmentsByIdQuery request,
            CancellationToken cancellationToken)
        {
            var customerSegments = await _customerSegmentsRepository.FindByIdAsync(request.Id, cancellationToken);
            if (customerSegments is null)
            {
                throw new NotFoundException($"Could not find CustomerSegments '{request.Id}'");
            }
            return _mapper.CustomerSegmentsToCustomerSegmentsDto(customerSegments);
        }
    }
}