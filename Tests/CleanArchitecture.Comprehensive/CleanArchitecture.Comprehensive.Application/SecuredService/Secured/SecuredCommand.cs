using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.SecuredService.Secured
{
    [Authorize]
    public class SecuredCommand : IRequest, ICommand
    {
        public SecuredCommand()
        {
        }
    }
}