using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.DeleteDb2Entity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDb2EntityCommandHandler : IRequestHandler<DeleteDb2EntityCommand>
    {
        private readonly IDb2EntityRepository _db2EntityRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDb2EntityCommandHandler(IDb2EntityRepository db2EntityRepository)
        {
            _db2EntityRepository = db2EntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDb2EntityCommand request, CancellationToken cancellationToken)
        {
            var db2Entity = await _db2EntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (db2Entity is null)
            {
                throw new NotFoundException($"Could not find Db2Entity '{request.Id}'");
            }

            _db2EntityRepository.Remove(db2Entity);
        }
    }
}