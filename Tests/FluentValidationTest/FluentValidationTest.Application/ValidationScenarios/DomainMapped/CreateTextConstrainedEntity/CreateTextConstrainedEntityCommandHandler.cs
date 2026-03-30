using FluentValidationTest.Domain.Entities.ValidationScenarios.TextConstraints;
using FluentValidationTest.Domain.Repositories.ValidationScenarios.TextConstraints;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace FluentValidationTest.Application.ValidationScenarios.DomainMapped.CreateTextConstrainedEntity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTextConstrainedEntityCommandHandler : IRequestHandler<CreateTextConstrainedEntityCommand>
    {
        private readonly ITextConstrainedEntityRepository _textConstrainedEntityRepository;
        [IntentManaged(Mode.Merge)]
        public CreateTextConstrainedEntityCommandHandler(ITextConstrainedEntityRepository textConstrainedEntityRepository)
        {
            _textConstrainedEntityRepository = textConstrainedEntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(CreateTextConstrainedEntityCommand request, CancellationToken cancellationToken)
        {
            var textConstrainedEntity = new TextConstrainedEntity
            {
                ShortCode = request.ShortCode,
                DisplayName = request.DisplayName,
                Description = request.Description,
                RequiredName = request.RequiredName,
                OptionalNotes = request.OptionalNotes,
                NullButRequired = request.NullButRequired,
                DefaultIntButRequired = request.DefaultIntButRequired
            };

            _textConstrainedEntityRepository.Add(textConstrainedEntity);
        }
    }
}