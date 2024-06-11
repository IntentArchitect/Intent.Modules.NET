using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace ProxyServiceTests.Proxy.TMS.Application.ClientsServices.UpdateClient
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        [IntentManaged(Mode.Merge)]
        public UpdateClientCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            RuleFor(v => v.Name)
                .NotNull();
        }
    }
}