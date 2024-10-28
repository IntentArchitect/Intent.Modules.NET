using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace FastEndpointsTest.Application.ScalarCollectionReturnType.CommandWithCollectionReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandWithCollectionReturnValidator : AbstractValidator<CommandWithCollectionReturn>
    {
        [IntentManaged(Mode.Merge)]
        public CommandWithCollectionReturnValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}