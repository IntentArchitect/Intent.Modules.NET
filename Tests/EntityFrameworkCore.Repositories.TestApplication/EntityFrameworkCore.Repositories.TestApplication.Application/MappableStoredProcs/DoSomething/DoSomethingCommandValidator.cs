using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace EntityFrameworkCore.Repositories.TestApplication.Application.MappableStoredProcs.DoSomething
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DoSomethingCommandValidator : AbstractValidator<DoSomethingCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DoSomethingCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}