using FluentValidationTest.Domain.Common.Exceptions;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.TextConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.UpdateTextConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateTextConstrainedEntityCommandHandler : IRequestHandler<UpdateTextConstrainedEntityCommand>
    {
        private readonly ITextConstrainedEntityRepository _textConstrainedEntityRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateTextConstrainedEntityCommandHandler(ITextConstrainedEntityRepository textConstrainedEntityRepository)
        {
            _textConstrainedEntityRepository = textConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateTextConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var textConstrainedEntity = await _textConstrainedEntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (textConstrainedEntity is null)
            {
                throw new NotFoundException($"Could not find TextConstrainedEntity '{request.Id}'");
            }

            textConstrainedEntity.ShortCode = request.ShortCode;
            textConstrainedEntity.DisplayName = request.DisplayName;
            textConstrainedEntity.Description = request.Description;
            textConstrainedEntity.RequiredName = request.RequiredName;
            textConstrainedEntity.OptionalNotes = request.OptionalNotes;

        }
    }
}