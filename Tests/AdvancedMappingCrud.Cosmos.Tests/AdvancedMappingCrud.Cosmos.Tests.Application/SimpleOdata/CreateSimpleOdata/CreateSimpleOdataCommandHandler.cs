using System;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Entities;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Application.SimpleOdata.CreateSimpleOdata
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateSimpleOdataCommandHandler : IRequestHandler<CreateSimpleOdataCommand, string>
    {
        private readonly ISimpleOdataRepository _simpleOdataRepository;

        [IntentManaged(Mode.Merge)]
        public CreateSimpleOdataCommandHandler(ISimpleOdataRepository simpleOdataRepository)
        {
            _simpleOdataRepository = simpleOdataRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateSimpleOdataCommand request, CancellationToken cancellationToken)
        {
            var simpleOdata = new Domain.Entities.SimpleOdata
            {
                Name = request.Name,
                Surname = request.Surname
            };

            _simpleOdataRepository.Add(simpleOdata);
            await _simpleOdataRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return simpleOdata.Id;
        }
    }
}