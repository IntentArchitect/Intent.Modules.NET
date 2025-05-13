using AutoMapper;
using Intent.Modules.NET.Tests.Domain.Core.Common.Exceptions;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers;
using Intent.Modules.NET.Tests.Module2.Application.Contracts.MyCustomers.GetMyCustomerById;
using Intent.Modules.NET.Tests.Module2.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Intent.Modules.NET.Tests.Module2.Application.MyCustomers.GetMyCustomerById
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetMyCustomerByIdQueryHandler : IRequestHandler<GetMyCustomerByIdQuery, MyCustomerDto>
    {
        private readonly IMyCustomerRepository _myCustomerRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetMyCustomerByIdQueryHandler(IMyCustomerRepository myCustomerRepository, IMapper mapper)
        {
            _myCustomerRepository = myCustomerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<MyCustomerDto> Handle(GetMyCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var myCustomer = await _myCustomerRepository.FindByIdAsync(request.Id, cancellationToken);
            if (myCustomer is null)
            {
                throw new NotFoundException($"Could not find MyCustomer '{request.Id}'");
            }
            return myCustomer.MapToMyCustomerDto(_mapper);
        }
    }
}