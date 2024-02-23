using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.DerivedOfTS.GetDerivedOfTS
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetDerivedOfTSQueryValidator : AbstractValidator<GetDerivedOfTSQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetDerivedOfTSQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}