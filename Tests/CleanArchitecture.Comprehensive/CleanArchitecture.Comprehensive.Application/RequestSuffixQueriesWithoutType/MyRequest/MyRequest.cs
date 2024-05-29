using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixQueriesWithoutType.MyRequest
{
    public class MyRequest : IRequest<int>, IQuery
    {
        public MyRequest()
        {
        }
    }
}