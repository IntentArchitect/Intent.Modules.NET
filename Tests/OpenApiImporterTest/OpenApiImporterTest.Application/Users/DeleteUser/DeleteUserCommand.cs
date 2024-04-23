using Intent.RoslynWeaver.Attributes;
using MediatR;
using OpenApiImporterTest.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace OpenApiImporterTest.Application.Users.DeleteUser
{
    public class DeleteUserCommand : IRequest, ICommand
    {
        public DeleteUserCommand(string username)
        {
            Username = username;
        }

        public string Username { get; set; }
    }
}