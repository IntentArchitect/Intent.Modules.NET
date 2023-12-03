using CleanArchitecture.Dapr.Application.Common.Interfaces;
using CleanArchitecture.Dapr.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace CleanArchitecture.Dapr.Application.SecuredProxy.GetSecuredValue
{
    [Authorize]
    public class GetSecuredValueQuery : IRequest<int>, IQuery
    {
        public GetSecuredValueQuery()
        {
        }
    }
}