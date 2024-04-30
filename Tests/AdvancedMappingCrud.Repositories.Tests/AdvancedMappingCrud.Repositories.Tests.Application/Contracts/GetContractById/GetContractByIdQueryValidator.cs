using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Contracts.GetContractById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetContractByIdQueryValidator : AbstractValidator<GetContractByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetContractByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}