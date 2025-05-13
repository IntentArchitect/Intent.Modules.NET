using AutoMapper;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomers;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.MyCustomers.GetMyCustomers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMyCustomersQueryHandler : IRequestHandler<GetMyCustomersQuery, List<MyCustomerDto>>
    {
        private readonly IMyCustomerRepository _myCustomerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMyCustomersQueryHandler(IMyCustomerRepository myCustomerRepository, IMapper mapper)
        {
            _myCustomerRepository = myCustomerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<MyCustomerDto>> Handle(GetMyCustomersQuery request, CancellationToken cancellationToken)
        {
            var myCustomers = await _myCustomerRepository.FindAllAsync(cancellationToken);
            return myCustomers.MapToMyCustomerDtoList(_mapper);
        }
    }
}