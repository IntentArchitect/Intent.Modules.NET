using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateNumericConstrainedEntity
{
    public class CreateNumericConstrainedEntityCommand : IRequest, ICommand
    {
        public CreateNumericConstrainedEntityCommand(int age, int percentage, double score, decimal price, int? optionalThreshold, float exclusiveMinInclusiveMaxFloat, double inclusiveMinExclusiveMaxDouble, decimal exclusiveMinExclusiveMaxDecimal, float onlyMinExclusiveFloat, double onlyMaxExclusiveDouble, float inclusiveMinInclusiveMaxFloat, decimal negativeRangeDecimal, double narrowRangeDouble)
        {
            Age = age;
            Percentage = percentage;
            Score = score;
            Price = price;
            OptionalThreshold = optionalThreshold;
            ExclusiveMinInclusiveMaxFloat = exclusiveMinInclusiveMaxFloat;
            InclusiveMinExclusiveMaxDouble = inclusiveMinExclusiveMaxDouble;
            ExclusiveMinExclusiveMaxDecimal = exclusiveMinExclusiveMaxDecimal;
            OnlyMinExclusiveFloat = onlyMinExclusiveFloat;
            OnlyMaxExclusiveDouble = onlyMaxExclusiveDouble;
            InclusiveMinInclusiveMaxFloat = inclusiveMinInclusiveMaxFloat;
            NegativeRangeDecimal = negativeRangeDecimal;
            NarrowRangeDouble = narrowRangeDouble;
        }

        public int Age { get; set; }
        public int Percentage { get; set; }
        public double Score { get; set; }
        public decimal Price { get; set; }
        public int? OptionalThreshold { get; set; }
        public float ExclusiveMinInclusiveMaxFloat { get; set; }
        public double InclusiveMinExclusiveMaxDouble { get; set; }
        public decimal ExclusiveMinExclusiveMaxDecimal { get; set; }
        public float OnlyMinExclusiveFloat { get; set; }
        public double OnlyMaxExclusiveDouble { get; set; }
        public float InclusiveMinInclusiveMaxFloat { get; set; }
        public decimal NegativeRangeDecimal { get; set; }
        public double NarrowRangeDouble { get; set; }
    }
}