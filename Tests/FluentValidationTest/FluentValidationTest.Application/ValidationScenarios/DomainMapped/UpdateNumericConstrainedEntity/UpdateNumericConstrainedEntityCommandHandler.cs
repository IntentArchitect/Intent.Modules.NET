using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.NumericConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateNumericConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateNumericConstrainedEntityCommandHandler : IRequestHandler<UpdateNumericConstrainedEntityCommand>
    {
        private readonly INumericConstrainedEntityRepository _numericConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdateNumericConstrainedEntityCommandHandler(INumericConstrainedEntityRepository numericConstrainedEntityRepository)
        {
            _numericConstrainedEntityRepository = numericConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateNumericConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var numericConstrainedEntity = await _numericConstrainedEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (numericConstrainedEntity is null)
            {
                throw new NotFoundException($"Could not find NumericConstrainedEntity '{request.Id}'");
            }

            numericConstrainedEntity.Age = request.Age;
            numericConstrainedEntity.Percentage = request.Percentage;
            numericConstrainedEntity.Score = request.Score;
            numericConstrainedEntity.Price = request.Price;
            numericConstrainedEntity.OptionalThreshold = request.OptionalThreshold;
        }
    }
}