using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.GetUser
{
    public class GetUserQuery : IRequest<User>, IQuery
    {
        public GetUserQuery(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }
}