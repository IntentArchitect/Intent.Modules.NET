using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace CosmosDB.PrivateSetters.Application.Departments.DeleteDepartment
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteDepartmentCommandValidator : AbstractValidator<DeleteDepartmentCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteDepartmentCommandValidator()
        {
        }
    }
}