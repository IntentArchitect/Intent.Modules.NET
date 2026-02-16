using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.AdvancedMappingSystem.Clients.GetClientExtraFields
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClientExtraFieldsQueryValidator : AbstractValidator<GetClientExtraFieldsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClientExtraFieldsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Field1)
                .NotNull();

            RuleFor(v => v.Field2)
                .NotNull();
        }
    }
}