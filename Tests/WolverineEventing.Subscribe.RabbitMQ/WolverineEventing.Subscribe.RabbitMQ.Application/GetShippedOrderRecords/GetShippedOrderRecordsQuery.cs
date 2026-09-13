using Intent.RoslynWeaver.Attributes;
using WolverineEventing.Subscribe.RabbitMQ.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace WolverineEventing.Subscribe.RabbitMQ.Application.GetShippedOrderRecords
{
    public class GetShippedOrderRecordsQuery : IQuery
    {
        public GetShippedOrderRecordsQuery()
        {
        }
    }
}