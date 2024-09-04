using FluentValidation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.FluentValidation.CommandValidator", Version = "2.0")]

namespace AdvancedMappingCrud.RichDomain.Tests.Application.Products.DoManual
{
    [IntentManaged(Mode.Fully, Body = Mode.Merge)]
    public class DoManualCommandValidator : AbstractValidator<DoManualCommand>
    {
        [IntentManaged(Mode.Merge)]
        public DoManualCommandValidator()
        {
            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
        }
    }
}