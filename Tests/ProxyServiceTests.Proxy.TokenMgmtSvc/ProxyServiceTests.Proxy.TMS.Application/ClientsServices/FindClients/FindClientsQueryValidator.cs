using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.QueryValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.FindClients
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class FindClientsQueryValidator : AbstractValidator<FindClientsQuery>
    {
        [IntentManaged(Mode.Merge)]
        public FindClientsQueryValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}