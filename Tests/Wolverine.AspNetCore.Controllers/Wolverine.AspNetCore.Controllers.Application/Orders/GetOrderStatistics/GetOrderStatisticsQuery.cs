using Intent.RoslynWeaver.Attributes;
using Wolverine.AspNetCore.Controllers.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Wolverine.QueryModels", Version = "1.0")]

namespace Wolverine.AspNetCore.Controllers.Application.GetOrderStatistics
{
    public class GetOrderStatisticsQuery : IQuery
    {
        public GetOrderStatisticsQuery()
        {
        }
    }
}