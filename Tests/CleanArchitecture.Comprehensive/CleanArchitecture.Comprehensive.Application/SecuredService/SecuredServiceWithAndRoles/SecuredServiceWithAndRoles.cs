using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.SecuredService.SecuredServiceWithAndRoles
{
    [Authorize(Roles = "Admin,One")]
    [Authorize(Roles = "Admin,Two")]
    public class SecuredServiceWithAndRoles : IRequest, ICommand
    {
        public SecuredServiceWithAndRoles()
        {
        }
    }
}