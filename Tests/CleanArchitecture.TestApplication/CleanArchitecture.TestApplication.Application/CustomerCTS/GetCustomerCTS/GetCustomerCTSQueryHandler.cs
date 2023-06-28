using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS.GetCustomerCTS
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerCTSQueryHandler : IRequestHandler<GetCustomerCTSQuery, List<CustomerCTDto>>
    {
        private readonly ICustomerCTRepository _customerCTRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomerCTSQueryHandler(ICustomerCTRepository customerCTRepository, IMapper mapper)
        {
            _customerCTRepository = customerCTRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerCTDto>> Handle(GetCustomerCTSQuery request, CancellationToken cancellationToken)
        {
            var customerCTs = await _customerCTRepository.FindAllAsync(cancellationToken);
            return customerCTs.MapToCustomerCTDtoList(_mapper);
        }
    }
}