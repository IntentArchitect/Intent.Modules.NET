using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Users.GetUserById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}