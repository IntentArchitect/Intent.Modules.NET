using AzureFunctions.NET6.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AzureFunctions.NET6.Application.Ignores.QueryWithIgnoreInApi
{
    public class QueryWithIgnoreInApi : IRequest<bool>, IQuery
    {
        public QueryWithIgnoreInApi()
        {
        }
    }
}