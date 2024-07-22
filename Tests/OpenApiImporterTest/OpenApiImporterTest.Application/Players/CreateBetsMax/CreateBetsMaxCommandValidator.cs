using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Players.CreateBetsMax
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateBetsMaxCommandValidator : AbstractValidator<CreateBetsMaxCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateBetsMaxCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.PriceChange)
                .NotNull()
                .IsInEnum();

            RuleFor(v => v.Items)
                .NotNull();

            RuleFor(v => v.PlayerId)
                .NotNull();
        }
    }
}