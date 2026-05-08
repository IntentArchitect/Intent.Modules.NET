using FluentValidationTest.Application.Interfaces.ValidationScenarios.EnumToStringMapping;
using FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping;
using FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.EnumMapping;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace FluentValidationTest.Application.Implementation.ValidationScenarios.EnumToStringMapping
{
    [IntentManaged(Mode.Merge)]
    public class EnumToStringMappingService : IEnumToStringMappingService
    {
        private readonly IEnumToStringMappingRepository _enumToStringMappingRepository;

        [IntentManaged(Mode.Merge)]
        public EnumToStringMappingService(IEnumToStringMappingRepository enumToStringMappingRepository)
        {
            _enumToStringMappingRepository = enumToStringMappingRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task ProcessOrder(ProcessOrderDto dto, CancellationToken cancellationToken = default)
        {
            var order = new Domain.Entities.ValidationScenarios.EnumMapping.EnumToStringMapping
            {
                StatusText = dto.Status.ToString(),
                Notes = dto.Notes,
                ProcessText = dto.Process.ToString()
            };

            _enumToStringMappingRepository.Add(order);
        }
    }
}