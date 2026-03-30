using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.ExplicitRules.ValidateExplicitNumericRules
{
    public class ValidateExplicitNumericRulesCommand : IRequest, ICommand
    {
        public ValidateExplicitNumericRulesCommand(int minimumInt,
            int maximumInt,
            long boundedLong,
            decimal boundedDecimal,
            double boundedDouble)
        {
            MinimumInt = minimumInt;
            MaximumInt = maximumInt;
            BoundedLong = boundedLong;
            BoundedDecimal = boundedDecimal;
            BoundedDouble = boundedDouble;
        }

        public int MinimumInt { get; set; }
        public int MaximumInt { get; set; }
        public long BoundedLong { get; set; }
        public decimal BoundedDecimal { get; set; }
        public double BoundedDouble { get; set; }
    }
}