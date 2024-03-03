using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.OneToManySources.GetOneToManySourceOneToManyDests
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetOneToManySourceOneToManyDestsQueryValidator : AbstractValidator<GetOneToManySourceOneToManyDestsQuery>
    {
        [IntentManaged(Mode.Fully, Body = Mode.Merge, Signature = Mode.Merge)]
        public GetOneToManySourceOneToManyDestsQueryValidator()
        {
            ConfigureValidationRules();
        }

        [IntentManaged(Mode.Fully)]
        private void ConfigureValidationRules()
        {
        }
    }
}