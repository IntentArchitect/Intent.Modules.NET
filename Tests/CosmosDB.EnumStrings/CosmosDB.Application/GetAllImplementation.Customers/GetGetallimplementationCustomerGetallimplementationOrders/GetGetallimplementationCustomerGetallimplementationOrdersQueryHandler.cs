using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CosmosDB.Domain.Entities;
using CosmosDB.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.GetAllImplementation.Customers.GetGetallimplementationCustomerGetallimplementationOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetGetallimplementationCustomerGetallimplementationOrdersQueryHandler : IRequestHandler<GetGetallimplementationCustomerGetallimplementationOrdersQuery, List<GetallimplementationCustomerGetallimplementationOrderDto>>
    {
        private readonly IGetAllImplementationCustomerRepository _getAllImplementationCustomerRepository;
        private readonly IMapper _mapper;
        [IntentManaged(Mode.Merge)]
        public GetGetallimplementationCustomerGetallimplementationOrdersQueryHandler(IGetAllImplementationCustomerRepository getAllImplementationCustomerRepository, IMapper mapper)
        {
            _getAllImplementationCustomerRepository = getAllImplementationCustomerRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// 5978511809 - GetAllImplementation strategy thinks it should apply even for unmapped queries
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<GetallimplementationCustomerGetallimplementationOrderDto>> Handle(
            GetGetallimplementationCustomerGetallimplementationOrdersQuery request,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}