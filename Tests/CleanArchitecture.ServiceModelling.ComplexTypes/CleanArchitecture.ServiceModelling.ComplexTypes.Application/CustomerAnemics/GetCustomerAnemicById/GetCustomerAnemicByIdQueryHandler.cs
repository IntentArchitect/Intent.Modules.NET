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

namespace CleanArchitecture.ServiceModelling.ComplexTypes.Application.CustomerAnemics.GetCustomerAnemicById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetCustomerAnemicByIdQueryHandler : IRequestHandler<GetCustomerAnemicByIdQuery, CustomerAnemicDto>
    {
        private readonly ICustomerAnemicRepository _customerAnemicRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Ignore)]
        public GetCustomerAnemicByIdQueryHandler(ICustomerAnemicRepository customerAnemicRepository, IMapper mapper)
        {
            _customerAnemicRepository = customerAnemicRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<CustomerAnemicDto> Handle(
            GetCustomerAnemicByIdQuery request,
            CancellationToken cancellationToken)
        {
            var customerAnemic = await _customerAnemicRepository.FindByIdAsync(request.Id, cancellationToken);

            if (customerAnemic is null)
            {
                throw new NotFoundException($"Could not find CustomerAnemic '{request.Id}'");
            }
            return customerAnemic.MapToCustomerAnemicDto(_mapper);
        }
    }
}