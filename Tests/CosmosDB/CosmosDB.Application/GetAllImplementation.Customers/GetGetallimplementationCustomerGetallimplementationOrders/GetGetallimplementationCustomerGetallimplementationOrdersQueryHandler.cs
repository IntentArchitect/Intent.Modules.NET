using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace CosmosDB.Application.GetAllImplementation.Customers.GetGetallimplementationCustomerGetallimplementationOrders
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetGetallimplementationCustomerGetallimplementationOrdersQueryHandler : IRequestHandler<GetGetallimplementationCustomerGetallimplementationOrdersQuery, List<GetallimplementationCustomerGetallimplementationOrderDto>>
    {
        [IntentManaged(Mode.Merge)]
        public GetGetallimplementationCustomerGetallimplementationOrdersQueryHandler()
        {
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