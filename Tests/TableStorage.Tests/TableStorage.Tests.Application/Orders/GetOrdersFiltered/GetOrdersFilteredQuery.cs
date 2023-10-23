using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using TableStorage.Tests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace TableStorage.Tests.Application.Orders.GetOrdersFiltered
{
    public class GetOrdersFilteredQuery : IRequest<List<OrderDto>>, IQuery
    {
        public GetOrdersFilteredQuery(string partitionKey)
        {
            PartitionKey = partitionKey;
        }

        public string PartitionKey { get; set; }
    }
}