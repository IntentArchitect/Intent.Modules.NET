using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Stores.GetOrder
{
    public class GetOrderQuery : IRequest<Order>, IQuery
    {
        public GetOrderQuery(int orderId)
        {
            OrderId = orderId;
        }

        public int OrderId { get; set; }
    }
}