using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Entities;
using CleanArchitecture.Dapr.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CleanArchitecture.Dapr.Application.OldMappingSystem.Tags.CreateTag
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, string>
    {
        private readonly ITagRepository _tagRepository;

        [IntentManaged(Mode.Merge)]
        public CreateTagCommandHandler(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateTagCommand request, CancellationToken cancellationToken)
        {
            var newTag = new Tag
            {
                Name = request.Name,
            };

            _tagRepository.Add(newTag);
            await _tagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newTag.Id;
        }
    }
}