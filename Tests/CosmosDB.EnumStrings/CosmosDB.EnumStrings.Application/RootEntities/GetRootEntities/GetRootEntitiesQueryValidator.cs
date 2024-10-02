using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EnumStrings.Application.RootEntities.GetRootEntities
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetRootEntitiesQueryValidator : AbstractValidator<GetRootEntitiesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetRootEntitiesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}