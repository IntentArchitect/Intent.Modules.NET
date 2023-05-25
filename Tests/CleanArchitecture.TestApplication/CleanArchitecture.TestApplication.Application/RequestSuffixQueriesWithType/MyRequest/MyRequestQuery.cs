using CleanArchitecture.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.TestApplication.Application.RequestSuffixQueriesWithType.MyRequest
{
    public class MyRequestQuery : IRequest<int>, IQuery
    {
        public MyRequestQuery()
        {
        }
    }
}