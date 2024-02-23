using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.UpdateDtoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateDtoReturnCommandValidator : AbstractValidator<UpdateDtoReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateDtoReturnCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}