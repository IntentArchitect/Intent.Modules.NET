using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ImplicitAsync
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ImplicitAsyncCommandValidator : AbstractValidator<ImplicitAsyncCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ImplicitAsyncCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}