using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ValueObjects.Class.Application.TestEntities.GetTestEntityById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetTestEntityByIdQueryValidator : AbstractValidator<GetTestEntityByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetTestEntityByIdQueryValidator()
        {
        }
    }
}