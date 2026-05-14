using AdvancedMapping.Repositories.Mapperly.Tests.Application.Mappings.CustomerSegments;
using AdvancedMapping.Repositories.Mapperly.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMapping.Repositories.Mapperly.Tests.Application.CustomerSegments.GetCustomerSegments
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerSegmentsQueryHandler : IRequestHandler<GetCustomerSegmentsQuery, List<CustomerSegmentsDto>>
    {
        private readonly ICustomerSegmentsRepository _customerSegmentsRepository;
        private readonly CustomerSegmentsDtoMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetCustomerSegmentsQueryHandler(ICustomerSegmentsRepository customerSegmentsRepository,
            CustomerSegmentsDtoMapper mapper)
        {
            _customerSegmentsRepository = customerSegmentsRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerSegmentsDto>> Handle(
            GetCustomerSegmentsQuery request,
            CancellationToken cancellationToken)
        {
            var customerSegments = await _customerSegmentsRepository.FindAllAsync(cancellationToken);
            return _mapper.CustomerSegmentsToCustomerSegmentsDtoList(customerSegments.ToList());
        }
    }
}