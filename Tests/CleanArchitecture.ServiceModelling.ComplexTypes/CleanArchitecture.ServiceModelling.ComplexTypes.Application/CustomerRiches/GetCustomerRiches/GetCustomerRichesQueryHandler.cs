using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.GetCustomerRiches
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerRichesQueryHandler : IRequestHandler<GetCustomerRichesQuery, List<CustomerRichDto>>
    {
        private readonly ICustomerRichRepository _customerRichRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomerRichesQueryHandler(ICustomerRichRepository customerRichRepository, IMapper mapper)
        {
            _customerRichRepository = customerRichRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerRichDto>> Handle(
            GetCustomerRichesQuery request,
            CancellationToken cancellationToken)
        {
            var customerRiches = await _customerRichRepository.FindAllAsync(cancellationToken);
            return customerRiches.MapToCustomerRichDtoList(_mapper);
        }
    }
}