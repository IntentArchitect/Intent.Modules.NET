using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.DeleteSimpleOdata
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteSimpleOdataCommandHandler : IRequestHandler<DeleteSimpleOdataCommand>
    {
        private readonly ISimpleOdataRepository _simpleOdataRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteSimpleOdataCommandHandler(ISimpleOdataRepository simpleOdataRepository)
        {
            _simpleOdataRepository = simpleOdataRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteSimpleOdataCommand request, CancellationToken cancellationToken)
        {
            var simpleOdata = await _simpleOdataRepository.FindByIdAsync(request.Id, cancellationToken);
            if (simpleOdata is null)
            {
                throw new NotFoundException($"Could not find SimpleOdata '{request.Id}'");
            }

            _simpleOdataRepository.Remove(simpleOdata);
        }
    }
}