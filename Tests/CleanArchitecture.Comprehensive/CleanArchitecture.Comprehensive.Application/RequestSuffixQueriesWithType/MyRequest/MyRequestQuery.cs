using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithType.MyRequest
{
    public class MyRequestQuery : IRequest<int>, IQuery
    {
        public MyRequestQuery()
        {
        }
    }
}