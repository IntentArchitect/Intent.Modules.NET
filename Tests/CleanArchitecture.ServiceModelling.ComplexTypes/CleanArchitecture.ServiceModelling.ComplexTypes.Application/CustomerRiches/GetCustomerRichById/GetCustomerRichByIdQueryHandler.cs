using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Common.Exceptions;
using CleanArchitecture.ServiceModelling.ComplexTypes.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerRiches.GetCustomerRichById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerRichByIdQueryHandler : IRequestHandler<GetCustomerRichByIdQuery, CustomerRichDto>
    {
        private readonly ICustomerRichRepository _customerRichRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomerRichByIdQueryHandler(ICustomerRichRepository customerRichRepository, IMapper mapper)
        {
            _customerRichRepository = customerRichRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerRichDto> Handle(GetCustomerRichByIdQuery request, CancellationToken cancellationToken)
        {
            var customerRich = await _customerRichRepository.FindByIdAsync(request.Id, cancellationToken);

            if (customerRich is null)
            {
                throw new NotFoundException($"Could not find CustomerRich {request.Id}");
            }
            return customerRich.MapToCustomerRichDto(_mapper);
        }
    }
}