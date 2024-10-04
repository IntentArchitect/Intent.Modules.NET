using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.SeparatedEntityState.Tests.Application.Companies.DeleteCompany
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DeleteCompanyCommandValidator : AbstractValidator<DeleteCompanyCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DeleteCompanyCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}