using CleanArchitecture.Comprehensive.BlazorClient.HttpClients.Contracts.Services.Validation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: DefaultIntentManaged(Mode.Fully, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.Blazor.HttpClients.ServiceContract", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.BlazorClient.HttpClients
{
    public interface IValidationService : IDisposable
    {
        Task InboundValidationAsync(InboundValidationCommand command, CancellationToken cancellationToken = default);
        Task<DummyResultDto> InboundValidationAsync(string rangeStr, string minStr, string maxStr, int rangeInt, int minInt, int maxInt, string isRequired, string isRequiredEmpty, decimal decimalRange, decimal decimalMin, decimal decimalMax, string? stringOption, string? stringOptionNonEmpty, EnumDescriptions myEnum, string regexField, CancellationToken cancellationToken = default);
    }
}