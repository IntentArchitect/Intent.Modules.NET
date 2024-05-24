using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.TestDCUser
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class TestDCUserCommandValidator : AbstractValidator<TestDCUserCommand>
    {
        [IntentManaged(Mode.Merge)]
        public TestDCUserCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}