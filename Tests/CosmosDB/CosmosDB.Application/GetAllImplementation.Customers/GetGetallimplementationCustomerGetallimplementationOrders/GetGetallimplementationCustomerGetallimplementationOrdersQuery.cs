using System.Collections.Generic;
using CosmosDB.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CosmosDB.Application.GetAllImplementation.Customers.GetGetallimplementationCustomerGetallimplementationOrders
{
    /// <summary>
    /// 5978511809 - GetAllImplementation strategy thinks it should apply even for unmapped queries
    /// </summary>
    public class GetGetallimplementationCustomerGetallimplementationOrdersQuery : IRequest<List<GetallimplementationCustomerGetallimplementationOrderDto>>, IQuery
    {
        public GetGetallimplementationCustomerGetallimplementationOrdersQuery(string getallimplementationCustomerid)
        {
            GetallimplementationCustomerid = getallimplementationCustomerid;
        }

        public string GetallimplementationCustomerid { get; set; }
    }
}