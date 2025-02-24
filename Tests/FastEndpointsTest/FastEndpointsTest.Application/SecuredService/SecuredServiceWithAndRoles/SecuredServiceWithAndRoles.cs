using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Application.Common.Security;
using FastEndpointsTest.Application.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.SecuredService.SecuredServiceWithAndRoles
{
    [Authorize(Roles = $"{Permissions.RoleAdmin},{Permissions.RoleOne}")]
    [Authorize(Roles = $"{Permissions.RoleAdmin},{Permissions.RoleTwo}")]
    public class SecuredServiceWithAndRoles : IRequest, ICommand
    {
        public SecuredServiceWithAndRoles()
        {
        }
    }
}