using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.ValidationServiceInterface", Version = "1.0")]

namespace AzureFunctions.NET6.Application
{
    public interface IValidationService
    {
        Task Handle<TRequest>(TRequest request, CancellationToken cancellationToken = default);
    }
}