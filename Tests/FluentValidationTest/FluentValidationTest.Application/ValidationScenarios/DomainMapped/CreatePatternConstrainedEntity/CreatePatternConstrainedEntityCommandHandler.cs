using FluentValidationTest.Domain.Entities.ValidationScenarios.PatternConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.PatternConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreatePatternConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreatePatternConstrainedEntityCommandHandler : IRequestHandler<CreatePatternConstrainedEntityCommand>
    {
        private readonly IPatternConstrainedEntityRepository _patternConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreatePatternConstrainedEntityCommandHandler(IPatternConstrainedEntityRepository patternConstrainedEntityRepository)
        {
            _patternConstrainedEntityRepository = patternConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreatePatternConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var patternConstrainedEntity = new PatternConstrainedEntity
            {
                EmailAddress = request.EmailAddress,
                WebsiteUrl = request.WebsiteUrl,
                Slug = request.Slug,
                ReferenceNumber = request.ReferenceNumber,
                OptionalPatternText = request.OptionalPatternText
            };

            _patternConstrainedEntityRepository.Add(patternConstrainedEntity);
        }
    }
}