using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetLogout
{
    public class GetLogoutQuery : IRequest<int>, IQuery
    {
        public GetLogoutQuery()
        {
        }
    }
}