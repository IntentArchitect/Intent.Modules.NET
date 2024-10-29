using AzureFunctions.NET6.Application.Ignores.CommandWithIgnoreInApi;
using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AzureFunctions.NET6.Application.Validators.Ignores.CommandWithIgnoreInApi
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class CommandWithIgnoreInApiValidator : AbstractValidator<Application.Ignores.CommandWithIgnoreInApi.CommandWithIgnoreInApi>
    {
        [IntentManaged(Mode.Merge)]
        public CommandWithIgnoreInApiValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}