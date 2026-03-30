using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.PatternConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdatePatternConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdatePatternConstrainedEntityCommandHandler : IRequestHandler<UpdatePatternConstrainedEntityCommand>
    {
        private readonly IPatternConstrainedEntityRepository _patternConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public UpdatePatternConstrainedEntityCommandHandler(IPatternConstrainedEntityRepository patternConstrainedEntityRepository)
        {
            _patternConstrainedEntityRepository = patternConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdatePatternConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var patternConstrainedEntity = await _patternConstrainedEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (patternConstrainedEntity is null)
            {
                throw new NotFoundException($"Could not find PatternConstrainedEntity '{request.Id}'");
            }

            patternConstrainedEntity.EmailAddress = request.EmailAddress;
            patternConstrainedEntity.WebsiteUrl = request.WebsiteUrl;
            patternConstrainedEntity.Slug = request.Slug;
            patternConstrainedEntity.ReferenceNumber = request.ReferenceNumber;
            patternConstrainedEntity.OptionalPatternText = request.OptionalPatternText;
        }
    }
}