using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.ExplicitETags.CreateExplicitETag
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateExplicitETagCommandHandler : IRequestHandler<CreateExplicitETagCommand, string>
    {
        private readonly IExplicitETagRepository _explicitETagRepository;

        [IntentManaged(Mode.Merge)]
        public CreateExplicitETagCommandHandler(IExplicitETagRepository explicitETagRepository)
        {
            _explicitETagRepository = explicitETagRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateExplicitETagCommand request, CancellationToken cancellationToken)
        {
            var explicitETag = new ExplicitETag
            {
                Name = request.Name
            };

            _explicitETagRepository.Add(explicitETag);
            await _explicitETagRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return explicitETag.Id;
        }
    }
}