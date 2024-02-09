using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Redis.Om.Repositories.Domain.Entities;
using Redis.Om.Repositories.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Redis.Om.Repositories.Application.DerivedTypes.CreateDerivedType
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDerivedTypeCommandHandler : IRequestHandler<CreateDerivedTypeCommand, string>
    {
        private readonly IDerivedTypeRepository _derivedTypeRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDerivedTypeCommandHandler(IDerivedTypeRepository derivedTypeRepository)
        {
            _derivedTypeRepository = derivedTypeRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<string> Handle(CreateDerivedTypeCommand request, CancellationToken cancellationToken)
        {
            var derivedType = new DerivedType
            {
                DerivedName = request.DerivedName,
                BaseName = request.BaseName,
                DerivedTypeAggregate = new DerivedTypeAggregate
                {
                    AggregateName = request.DerivedTypeAggregate.AggregateName
                }
            };

            _derivedTypeRepository.Add(derivedType);
            await _derivedTypeRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return derivedType.Id;
        }
    }
}