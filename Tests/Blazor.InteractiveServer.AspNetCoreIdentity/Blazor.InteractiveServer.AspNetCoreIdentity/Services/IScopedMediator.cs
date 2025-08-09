using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.ScopedMediatorInterfaceTemplate", Version = "1.0")]

namespace Blazor.InteractiveServer.AspNetCoreIdentity.Services
{
    public interface IScopedMediator : ISender
    {
    }
}