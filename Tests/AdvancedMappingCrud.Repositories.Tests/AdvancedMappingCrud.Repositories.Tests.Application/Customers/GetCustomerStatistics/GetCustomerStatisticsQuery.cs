using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Customers.GetCustomerStatistics
{
    public class GetCustomerStatisticsQuery : IRequest<int>, IQuery
    {
        public GetCustomerStatisticsQuery(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; set; }
    }
}