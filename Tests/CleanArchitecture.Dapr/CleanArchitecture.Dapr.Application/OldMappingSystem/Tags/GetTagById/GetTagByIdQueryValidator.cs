using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.GetTagById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetTagByIdQueryValidator : AbstractValidator<GetTagByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetTagByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();
        }
    }
}