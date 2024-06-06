using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.TestNullablities.DeleteTestNullablity
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteTestNullablityCommandValidator : AbstractValidator<DeleteTestNullablityCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteTestNullablityCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}