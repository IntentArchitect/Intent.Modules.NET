using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUserAddresses
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class GetUserAddressesQueryValidator : AbstractValidator<GetUserAddressesQuery>
    {
        [IntentManaged(Mode.Merge)]
        public GetUserAddressesQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}