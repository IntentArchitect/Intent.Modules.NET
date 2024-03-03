using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.GetManyToOneDestById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetManyToOneDestByIdQueryValidator : AbstractValidator<GetManyToOneDestByIdQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetManyToOneDestByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}