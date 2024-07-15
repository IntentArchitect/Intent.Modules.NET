using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrderByRefNo
{
    public class GetOrderByRefNoQuery : IRequest<OrderDto>, IQuery
    {
        public GetOrderByRefNoQuery(string refNo, string external)
        {
            RefNo = refNo;
            External = external;
        }

        public string RefNo { get; set; }
        public string External { get; set; }
    }
}