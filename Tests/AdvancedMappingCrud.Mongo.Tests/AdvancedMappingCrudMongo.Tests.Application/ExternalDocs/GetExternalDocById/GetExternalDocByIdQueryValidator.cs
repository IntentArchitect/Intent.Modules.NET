using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetExternalDocByIdQueryValidator : AbstractValidator<GetExternalDocByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetExternalDocByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}