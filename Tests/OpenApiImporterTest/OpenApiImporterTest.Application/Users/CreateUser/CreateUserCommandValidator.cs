using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Users.CreateUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public CreateUserCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Username)
                .NotNull();

            RuleFor(v => v.FirstName)
                .NotNull();

            RuleFor(v => v.LastName)
                .NotNull();

            RuleFor(v => v.Email)
                .NotNull();

            RuleFor(v => v.Password)
                .NotNull();

            RuleFor(v => v.Phone)
                .NotNull();
        }
    }
}