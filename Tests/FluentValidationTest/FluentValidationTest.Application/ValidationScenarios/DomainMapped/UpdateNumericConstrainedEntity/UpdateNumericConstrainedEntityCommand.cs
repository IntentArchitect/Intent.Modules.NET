using FluentValidationTest.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateNumericConstrainedEntity
{
    public class UpdateNumericConstrainedEntityCommand : IRequest, ICommand
    {
        public UpdateNumericConstrainedEntityCommand(Guid id, int age, int percentage, double score, decimal price, int? optionalThreshold)
        {
            Id = id;
            Age = age;
            Percentage = percentage;
            Score = score;
            Price = price;
            OptionalThreshold = optionalThreshold;
        }

        public Guid Id { get; set; }
        public int Age { get; set; }
        public int Percentage { get; set; }
        public double Score { get; set; }
        public decimal Price { get; set; }
        public int? OptionalThreshold { get; set; }
    }
}