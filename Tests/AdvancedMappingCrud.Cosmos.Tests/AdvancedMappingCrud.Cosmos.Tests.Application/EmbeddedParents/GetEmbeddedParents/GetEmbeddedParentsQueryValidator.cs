using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.EmbeddedParents.GetEmbeddedParents
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetEmbeddedParentsQueryValidator : AbstractValidator<GetEmbeddedParentsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetEmbeddedParentsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // Implement custom validation logic here if required
        }
    }
}