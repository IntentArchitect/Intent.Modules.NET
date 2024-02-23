using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.ClassContainers.GetClassContainers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetClassContainersQueryValidator : AbstractValidator<GetClassContainersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetClassContainersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}