using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrudMongo.Tests.Application.ExternalDocs.GetExternalDocs
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetExternalDocsQueryValidator : AbstractValidator<GetExternalDocsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetExternalDocsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}