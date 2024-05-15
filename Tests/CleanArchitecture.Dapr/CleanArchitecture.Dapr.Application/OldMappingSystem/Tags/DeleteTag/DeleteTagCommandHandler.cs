using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.DeleteTag
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand>
    {
        private readonly ITagRepository _tagRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteTagCommandHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteTagCommand request, CancellationToken cancellationToken)
        {
            var existingTag = await _tagRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingTag is null)
            {
                throw new NotFoundException($"Could not find Tag '{request.Id}'");
            }

            _tagRepository.Remove(existingTag);

        }
    }
}