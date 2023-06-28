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

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.GetCustomerAnemics
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerAnemicsQueryHandler : IRequestHandler<GetCustomerAnemicsQuery, List<CustomerAnemicDto>>
    {
        private readonly ICustomerAnemicRepository _customerAnemicRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomerAnemicsQueryHandler(ICustomerAnemicRepository customerAnemicRepository, IMapper mapper)
        {
            _customerAnemicRepository = customerAnemicRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerAnemicDto>> Handle(
            GetCustomerAnemicsQuery request,
            CancellationToken cancellationToken)
        {
            var customerAnemics = await _customerAnemicRepository.FindAllAsync(cancellationToken);
            return customerAnemics.MapToCustomerAnemicDtoList(_mapper);
        }
    }
}