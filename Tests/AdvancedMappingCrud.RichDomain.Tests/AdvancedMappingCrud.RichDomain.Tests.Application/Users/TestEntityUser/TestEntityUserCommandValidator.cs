using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestEntityUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestEntityUserCommandValidator : AbstractValidator<TestEntityUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestEntityUserCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}