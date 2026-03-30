using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateNumericConstrainedEntity
{
    public class CreateNumericConstrainedEntityCommand : IRequest, ICommand
    {
        public CreateNumericConstrainedEntityCommand(int age, int percentage, double score, decimal price, int? optionalThreshold)
        {
            Age = age;
            Percentage = percentage;
            Score = score;
            Price = price;
            OptionalThreshold = optionalThreshold;
        }

        public int Age { get; set; }
        public int Percentage { get; set; }
        public double Score { get; set; }
        public decimal Price { get; set; }
        public int? OptionalThreshold { get; set; }
    }
}