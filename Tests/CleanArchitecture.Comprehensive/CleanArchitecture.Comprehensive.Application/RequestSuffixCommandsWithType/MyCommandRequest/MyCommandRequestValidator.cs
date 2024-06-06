using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithType.MyCommandRequest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MyCommandRequestValidator : AbstractValidator<MyCommandRequest>
    {
        [IntentManaged(Mode.Merge)]
        public MyCommandRequestValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}