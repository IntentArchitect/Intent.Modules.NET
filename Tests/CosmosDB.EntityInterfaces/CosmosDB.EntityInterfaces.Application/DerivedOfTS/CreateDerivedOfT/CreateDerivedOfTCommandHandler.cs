using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CosmosDB.EntityInterfaces.Domain.Entities;
using CosmosDB.EntityInterfaces.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace CosmosDB.EntityInterfaces.Application.DerivedOfTS.CreateDerivedOfT
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDerivedOfTCommandHandler : IRequestHandler<CreateDerivedOfTCommand, string>
    {
        private readonly IDerivedOfTRepository _derivedOfTRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDerivedOfTCommandHandler(IDerivedOfTRepository derivedOfTRepository)
        {
            _derivedOfTRepository = derivedOfTRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDerivedOfTCommand request, CancellationToken cancellationToken)
        {
            var newDerivedOfT = new DerivedOfT
            {
                DerivedAttribute = request.DerivedAttribute,
                GenericAttribute = request.GenericAttribute,
            };

            _derivedOfTRepository.Add(newDerivedOfT);
            await _derivedOfTRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newDerivedOfT.Id;
        }
    }
}