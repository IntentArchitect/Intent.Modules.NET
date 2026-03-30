using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateCrossFieldRules
{
    public class ValidateCrossFieldRulesCommand : IRequest, ICommand
    {
        public ValidateCrossFieldRulesCommand(string primaryValue,
            string secondaryValue,
            int comparisonNumber,
            int otherNumber)
        {
            PrimaryValue = primaryValue;
            SecondaryValue = secondaryValue;
            ComparisonNumber = comparisonNumber;
            OtherNumber = otherNumber;
        }

        public string PrimaryValue { get; set; }
        public string SecondaryValue { get; set; }
        public int ComparisonNumber { get; set; }
        public int OtherNumber { get; set; }
    }
}