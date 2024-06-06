using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyRequest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MyRequestCommandValidator : AbstractValidator<MyRequestCommand>
    {
        [IntentManaged(Mode.Merge)]
        public MyRequestCommandValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}