using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUserAddressById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUserAddressByIdQueryValidator : AbstractValidator<GetUserAddressByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserAddressByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}