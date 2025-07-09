using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.TestApplication.Domain.Entities.Aggregational;
using Entities.PrivateSetters.TestApplication.Domain.Repositories.Aggregational;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Entities.PrivateSetters.TestApplication.Application.ManyToOneSources.CreateManyToOneSource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateManyToOneSourceCommandHandler : IRequestHandler<CreateManyToOneSourceCommand, Guid>
    {
        private readonly IManyToOneSourceRepository _manyToOneSourceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateManyToOneSourceCommandHandler(IManyToOneSourceRepository manyToOneSourceRepository)
        {
            _manyToOneSourceRepository = manyToOneSourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateManyToOneSourceCommand request, CancellationToken cancellationToken)
        {
#warning No supported convention for populating "manyToOneDest" parameter
            var newManyToOneSource = new ManyToOneSource(request.Attribute, manyToOneDest: default);

            _manyToOneSourceRepository.Add(newManyToOneSource);
            await _manyToOneSourceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newManyToOneSource.Id;
        }
    }
}