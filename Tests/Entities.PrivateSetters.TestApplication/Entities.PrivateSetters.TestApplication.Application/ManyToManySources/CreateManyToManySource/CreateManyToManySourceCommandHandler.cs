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

namespace Entities.PrivateSetters.TestApplication.Application.ManyToManySources.CreateManyToManySource
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateManyToManySourceCommandHandler : IRequestHandler<CreateManyToManySourceCommand, Guid>
    {
        private readonly IManyToManySourceRepository _manyToManySourceRepository;

        [IntentManaged(Mode.Merge)]
        public CreateManyToManySourceCommandHandler(IManyToManySourceRepository manyToManySourceRepository)
        {
            _manyToManySourceRepository = manyToManySourceRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateManyToManySourceCommand request, CancellationToken cancellationToken)
        {
#warning No supported convention for populating "manyToManyDests" parameter
            var newManyToManySource = new ManyToManySource(request.Attribute, manyToManyDests: default);

            _manyToManySourceRepository.Add(newManyToManySource);
            await _manyToManySourceRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return newManyToManySource.Id;
        }
    }
}