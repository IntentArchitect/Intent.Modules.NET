using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.Departments.GetDepartments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDepartmentsQueryValidator : AbstractValidator<GetDepartmentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDepartmentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}