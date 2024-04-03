using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using SqlServerImporterTests.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace SqlServerImporterTests.Application.Orders.GetCustomerOrders
{
    public class GetCustomerOrdersQuery : IRequest<List<CustomerOrderDto>>, IQuery
    {
        public GetCustomerOrdersQuery(Guid customerID)
        {
            CustomerID = customerID;
        }

        public Guid CustomerID { get; set; }
    }
}