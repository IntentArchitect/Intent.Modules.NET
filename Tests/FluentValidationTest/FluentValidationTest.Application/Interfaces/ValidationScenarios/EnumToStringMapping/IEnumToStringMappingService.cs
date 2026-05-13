using FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace FluentValidationTest.Application.Interfaces.ValidationScenarios.EnumToStringMapping
{
    public interface IEnumToStringMappingService
    {
        Task ProcessOrder(ProcessOrderDto dto, CancellationToken cancellationToken = default);
    }
}