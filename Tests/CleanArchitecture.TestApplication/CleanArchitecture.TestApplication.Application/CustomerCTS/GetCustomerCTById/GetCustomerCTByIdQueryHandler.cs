using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.TestApplication.Domain.Common.Exceptions;
using CleanArchitecture.TestApplication.Domain.Repositories.ComplexTypes;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.CustomerCTS.GetCustomerCTById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerCTByIdQueryHandler : IRequestHandler<GetCustomerCTByIdQuery, CustomerCTDto>
    {
        private readonly ICustomerCTRepository _customerCTRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomerCTByIdQueryHandler(ICustomerCTRepository customerCTRepository, IMapper mapper)
        {
            _customerCTRepository = customerCTRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerCTDto> Handle(GetCustomerCTByIdQuery request, CancellationToken cancellationToken)
        {
            var customerCT = await _customerCTRepository.FindByIdAsync(request.Id, cancellationToken);

            if (customerCT is null)
            {
                throw new NotFoundException($"Could not find CustomerCT {request.Id}");
            }
            return customerCT.MapToCustomerCTDto(_mapper);
        }
    }
}