using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.UpdateExplicitETag
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateExplicitETagCommandHandler : IRequestHandler<UpdateExplicitETagCommand>
    {
        private readonly IExplicitETagRepository _explicitETagRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateExplicitETagCommandHandler(IExplicitETagRepository explicitETagRepository)
        {
            _explicitETagRepository = explicitETagRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateExplicitETagCommand request, CancellationToken cancellationToken)
        {
            var explicitETag = await _explicitETagRepository.FindByIdAsync(request.Id, cancellationToken);
            if (explicitETag is null)
            {
                throw new NotFoundException($"Could not find ExplicitETag '{request.Id}'");
            }

            explicitETag.Name = request.Name;
            explicitETag.ETag = request.ETag;

            _explicitETagRepository.Update(explicitETag);
        }
    }
}