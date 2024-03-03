using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneDests.GetManyToOneDests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetManyToOneDestsQueryValidator : AbstractValidator<GetManyToOneDestsQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetManyToOneDestsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}