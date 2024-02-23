using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.ClassContainers.GetClassContainerById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClassContainerByIdQueryValidator : AbstractValidator<GetClassContainerByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClassContainerByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Id)
                .NotNull();

            RuleFor(v => v.ClassPartitionKey)
                .NotNull();
        }
    }
}