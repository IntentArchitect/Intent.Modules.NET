using FastEndpointsTest.Application.Common.Interfaces;
using FastEndpointsTest.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FastEndpointsTest.Application.SecuredService.Secured
{
    [Authorize]
    public class SecuredCommand : IRequest, ICommand
    {
        public SecuredCommand()
        {
        }
    }
}