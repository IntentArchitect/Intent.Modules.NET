using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.Jobs.MyTimedJob
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MyTimedJobCommandValidator : AbstractValidator<MyTimedJobCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MyTimedJobCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}