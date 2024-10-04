using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Users.TestVOUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestVOUserCommandValidator : AbstractValidator<TestVOUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestVOUserCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}