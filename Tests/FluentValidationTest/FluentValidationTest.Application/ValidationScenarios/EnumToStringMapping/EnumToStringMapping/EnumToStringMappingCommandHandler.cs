using FluentValidationTest.Domain.Entities.ValidationScenarios.EnumMapping;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.EnumMapping;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.EnumToStringMapping.EnumToStringMapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class EnumToStringMappingCommandHandler : IRequestHandler<EnumToStringMappingCommand>
    {
        private readonly IEnumToStringMappingRepository _enumToStringMappingRepository;

        [IntentManaged(Mode.Merge)]
        public EnumToStringMappingCommandHandler(IEnumToStringMappingRepository enumToStringMappingRepository)
        {
            _enumToStringMappingRepository = enumToStringMappingRepository;
        }

        /// <summary>
        /// CreateOrderCommand DTO with enum-to-string mapping scenario.
        /// 
        /// Test Case: Verify enum field (Status) generates IsInEnum() validator.
        /// Also verify string field (Notes) generates MaximumLength(100) validator.
        /// 
        /// Domain Entity Target: Order (OrderStatus enum mapped to Order.StatusText string)
        /// </summary>
        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(EnumToStringMappingCommand request, CancellationToken cancellationToken)
        {
            var order = new Domain.Entities.ValidationScenarios.EnumMapping.EnumToStringMapping
            {
                StatusText = request.Status.ToString(),
                Notes = request.Notes,
                ProcessText = request.Process.ToString()
            };

            _enumToStringMappingRepository.Add(order);
        }
    }
}