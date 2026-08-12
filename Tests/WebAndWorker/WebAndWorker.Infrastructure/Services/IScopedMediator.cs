using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Blazor.Templates.Server.ScopedMediatorInterfaceTemplate", Version = "1.0")]

namespace WebAndWorker.Infrastructure.Services
{
    public interface IScopedMediator : ISender
    {
    }
}