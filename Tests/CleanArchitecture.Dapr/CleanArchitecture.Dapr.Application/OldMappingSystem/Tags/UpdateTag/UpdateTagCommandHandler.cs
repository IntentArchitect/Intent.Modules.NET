using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Exceptions;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.UpdateTag
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateTagCommandHandler : IRequestHandler<UpdateTagCommand>
    {
        private readonly ITagRepository _tagRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateTagCommandHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateTagCommand request, CancellationToken cancellationToken)
        {
            var existingTag = await _tagRepository.FindByIdAsync(request.Id, cancellationToken);
            if (existingTag is null)
            {
                throw new NotFoundException($"Could not find Tag '{request.Id}'");
            }

            existingTag.Name = request.Name;

            _tagRepository.Update(existingTag);

        }
    }
}