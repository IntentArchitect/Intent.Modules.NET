using CleanArchitecture.TestApplication.BlazorClient.HttpClients.Contracts.Services.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.TestApplication.BlazorClient.HttpClients
{
    public interface IValidationService : IDisposable
    {
        Task<ValidatedResultDto> ResultValidationsAsync(CancellationToken cancellationToken = default);
    }
}