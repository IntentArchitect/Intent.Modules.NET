using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users.DeleteUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteUserCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}