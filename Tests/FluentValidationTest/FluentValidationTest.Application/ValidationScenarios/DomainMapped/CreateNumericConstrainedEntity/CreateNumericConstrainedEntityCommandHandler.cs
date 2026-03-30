using FluentValidationTest.Domain.Entities.ValidationScenarios.NumericConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.NumericConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateNumericConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateNumericConstrainedEntityCommandHandler : IRequestHandler<CreateNumericConstrainedEntityCommand>
    {
        private readonly INumericConstrainedEntityRepository _numericConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateNumericConstrainedEntityCommandHandler(INumericConstrainedEntityRepository numericConstrainedEntityRepository)
        {
            _numericConstrainedEntityRepository = numericConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateNumericConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var numericConstrainedEntity = new NumericConstrainedEntity
            {
                Age = request.Age,
                Percentage = request.Percentage,
                Score = request.Score,
                Price = request.Price,
                OptionalThreshold = request.OptionalThreshold
            };

            _numericConstrainedEntityRepository.Add(numericConstrainedEntity);
        }
    }
}