using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.FluentValidation.Dtos.ValidationServiceInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Common.Validation
{
    public interface IValidationService
    {
        Task Handle<TRequest>(TRequest request, CancellationToken cancellationToken = default);
    }
}