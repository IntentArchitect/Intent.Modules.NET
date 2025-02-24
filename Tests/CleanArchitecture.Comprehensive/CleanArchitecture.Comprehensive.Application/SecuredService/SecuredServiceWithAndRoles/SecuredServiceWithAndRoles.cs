using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.Common.Security;
using CleanArchitecture.Comprehensive.Application.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.SecuredService.SecuredServiceWithAndRoles
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