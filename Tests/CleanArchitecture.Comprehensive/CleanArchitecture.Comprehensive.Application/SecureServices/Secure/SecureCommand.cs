using CleanArchitecture.Comprehensive.Application.Common.Interfaces;
using CleanArchitecture.Comprehensive.Application.Common.Security;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace CleanArchitecture.Comprehensive.Application.SecureServices.Secure
{
    [Authorize]
    public class SecureCommand : IRequest, ICommand
    {
        public SecureCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}