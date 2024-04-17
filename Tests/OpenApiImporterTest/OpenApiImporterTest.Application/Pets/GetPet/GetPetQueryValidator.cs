using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace OpenApiImporterTest.Application.Pets.GetPet
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetPetQueryValidator : AbstractValidator<GetPetQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetPetQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}