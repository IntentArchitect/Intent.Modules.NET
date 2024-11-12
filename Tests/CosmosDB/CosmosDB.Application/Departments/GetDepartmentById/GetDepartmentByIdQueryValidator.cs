using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Departments.GetDepartmentById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDepartmentByIdQueryValidator : AbstractValidator<GetDepartmentByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDepartmentByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}