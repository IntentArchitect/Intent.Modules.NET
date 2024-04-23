using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetUserLogout
{
    public class GetUserLogoutQuery : IRequest<int>, IQuery
    {
        public GetUserLogoutQuery()
        {
        }
    }
}