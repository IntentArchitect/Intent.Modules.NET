using AzureFunctions.NET8.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET8.Application.Ignores.QueryWithIgnoreInApi
{
    public class QueryWithIgnoreInApi : IRequest<bool>, IQuery
    {
        public QueryWithIgnoreInApi()
        {
        }
    }
}