using AdvancedMappingCrudMongo.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.Orders.GetOrderByRef
{
    public class GetOrderByRefQuery : IRequest<OrderDto>, IQuery
    {
        public GetOrderByRefQuery(string? refNo, string? externalRefNo)
        {
            RefNo = refNo;
            ExternalRefNo = externalRefNo;
        }

        public string? RefNo { get; set; }
        public string? ExternalRefNo { get; set; }
    }
}