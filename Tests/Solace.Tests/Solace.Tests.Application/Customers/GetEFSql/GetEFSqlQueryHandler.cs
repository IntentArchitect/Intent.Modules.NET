using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Solace.Tests.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace Solace.Tests.Application.Customers.GetEFSql
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetEFSqlQueryHandler : IRequestHandler<GetEFSqlQuery, List<CustomerDto>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public GetEFSqlQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<CustomerDto>> Handle(GetEFSqlQuery request, CancellationToken cancellationToken)
        {
            var result = await _customerRepository.SearchSqlEFAsync(cancellationToken);
            return result.MapToCustomerDtoList(_mapper);
        }
    }
}