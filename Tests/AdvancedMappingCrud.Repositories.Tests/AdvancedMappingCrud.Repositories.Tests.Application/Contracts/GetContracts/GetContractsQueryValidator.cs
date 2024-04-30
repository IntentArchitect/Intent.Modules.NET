using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.GetContracts
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetContractsQueryValidator : AbstractValidator<GetContractsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetContractsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}