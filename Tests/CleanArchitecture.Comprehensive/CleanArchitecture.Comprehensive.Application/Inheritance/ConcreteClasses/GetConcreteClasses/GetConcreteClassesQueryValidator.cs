using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.Comprehensive.Application.Inheritance.ConcreteClasses.GetConcreteClasses
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetConcreteClassesQueryValidator : AbstractValidator<GetConcreteClassesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetConcreteClassesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}