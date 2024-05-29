using System;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.RequestSuffixCommandsWithoutType.MyRequest
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class MyRequestValidator : AbstractValidator<MyRequest>
    {
        [IntentManaged(Mode.Merge)]
        public MyRequestValidator()
        {
            ConfigureValidationRules();

        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}