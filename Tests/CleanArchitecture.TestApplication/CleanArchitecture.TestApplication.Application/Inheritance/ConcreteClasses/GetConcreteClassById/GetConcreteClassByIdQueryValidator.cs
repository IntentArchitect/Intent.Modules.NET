using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CleanArchitecture.TestApplication.Application.Inheritance.ConcreteClasses.GetConcreteClassById
{
    public class GetConcreteClassByIdQueryValidator : AbstractValidator<GetConcreteClassByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetConcreteClassByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}