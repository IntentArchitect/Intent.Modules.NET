using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace IntegrationTesting.Tests.Application.DtoReturns.CreateDtoReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateDtoReturnCommandValidator : AbstractValidator<CreateDtoReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateDtoReturnCommandValidator()
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