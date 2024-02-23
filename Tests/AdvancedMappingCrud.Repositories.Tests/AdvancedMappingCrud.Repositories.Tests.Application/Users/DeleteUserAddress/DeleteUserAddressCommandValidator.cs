using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.DeleteUserAddress
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteUserAddressCommandValidator : AbstractValidator<DeleteUserAddressCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteUserAddressCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}