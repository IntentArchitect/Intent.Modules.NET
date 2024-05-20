using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.GetUsers
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUsersQueryValidator : AbstractValidator<GetUsersQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUsersQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}