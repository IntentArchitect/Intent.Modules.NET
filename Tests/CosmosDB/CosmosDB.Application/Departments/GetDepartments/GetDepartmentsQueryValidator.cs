using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.Application.Departments.GetDepartments
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDepartmentsQueryValidator : AbstractValidator<GetDepartmentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDepartmentsQueryValidator()
        {
        }
    }
}