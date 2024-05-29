using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.AsyncOperationsClasses.ImplicitAsyncWithReturn
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class ImplicitAsyncWithReturnCommandValidator : AbstractValidator<ImplicitAsyncWithReturnCommand>
    {
        [IntentManaged(Mode.Merge)]
        public ImplicitAsyncWithReturnCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}