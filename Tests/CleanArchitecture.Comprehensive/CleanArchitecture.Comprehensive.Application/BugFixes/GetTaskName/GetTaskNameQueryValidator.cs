using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.BugFixes.GetTaskName
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetTaskNameQueryValidator : AbstractValidator<GetTaskNameQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetTaskNameQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}