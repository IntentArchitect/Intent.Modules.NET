using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.Nullability.ValidateNullability
{
    public class ValidateNullabilityCommand : IRequest, ICommand
    {
        public ValidateNullabilityCommand(string requiredString,
            string? optionalString,
            int requiredInt,
            int? optionalInt,
            Guid requiredGuidValue,
            Guid? optionalGuidValue,
            DateTime requiredDateValue,
            DateTime? optionalDateValue,
            SimplePayloadDto requiredPayload,
            SimplePayloadDto? optionalPayload)
        {
            RequiredString = requiredString;
            OptionalString = optionalString;
            RequiredInt = requiredInt;
            OptionalInt = optionalInt;
            RequiredGuidValue = requiredGuidValue;
            OptionalGuidValue = optionalGuidValue;
            RequiredDateValue = requiredDateValue;
            OptionalDateValue = optionalDateValue;
            RequiredPayload = requiredPayload;
            OptionalPayload = optionalPayload;
        }

        public string RequiredString { get; set; }
        public string? OptionalString { get; set; }
        public int RequiredInt { get; set; }
        public int? OptionalInt { get; set; }
        public Guid RequiredGuidValue { get; set; }
        public Guid? OptionalGuidValue { get; set; }
        public DateTime RequiredDateValue { get; set; }
        public DateTime? OptionalDateValue { get; set; }
        public SimplePayloadDto RequiredPayload { get; set; }
        public SimplePayloadDto? OptionalPayload { get; set; }
    }
}