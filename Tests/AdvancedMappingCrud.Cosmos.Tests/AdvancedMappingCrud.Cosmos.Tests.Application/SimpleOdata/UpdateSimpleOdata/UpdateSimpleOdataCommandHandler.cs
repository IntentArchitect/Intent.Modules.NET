using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.UpdateSimpleOdata
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class UpdateSimpleOdataCommandHandler : IRequestHandler<UpdateSimpleOdataCommand>
    {
        private readonly ISimpleOdataRepository _simpleOdataRepository;

        [IntentManaged(Mode.Merge)]
        public UpdateSimpleOdataCommandHandler(ISimpleOdataRepository simpleOdataRepository)
        {
            _simpleOdataRepository = simpleOdataRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(UpdateSimpleOdataCommand request, CancellationToken cancellationToken)
        {
            var simpleOdata = await _simpleOdataRepository.FindByIdAsync(request.Id, cancellationToken);
            if (simpleOdata is null)
            {
                throw new NotFoundException($"Could not find SimpleOdata '{request.Id}'");
            }

            simpleOdata.Name = request.Name;
            simpleOdata.Surname = request.Surname;

            _simpleOdataRepository.Update(simpleOdata);
        }
    }
}