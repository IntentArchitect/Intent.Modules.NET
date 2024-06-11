using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.FindClientById
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FindClientByIdQueryValidator : AbstractValidator<FindClientByIdQuery>
    {
        [IntentManaged(Mode.Merge)]
        public FindClientByIdQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}