using CleanArchitecture.Comprehensive.Application.Common.Validation;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BasicMappingMapToValueObjects.CreateSubmission
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateSubmissionCommandValidator : AbstractValidator<CreateSubmissionCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateSubmissionCommandValidator(IValidatorProvider provider)
        {
            ConfigureValidationRules(provider);
        }

        private void ConfigureValidationRules(IValidatorProvider provider)
        {
            RuleFor(v => v.SubmissionType)
                .NotNull();

            RuleFor(v => v.Items)
                .NotNull()
                .ForEach(x => x.SetValidator(provider.GetValidator<CreateSubmissionItemDto>()!));
        }
    }
}