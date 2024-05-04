using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Common.Exceptions;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.DeleteDb1Entity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DeleteDb1EntityCommandHandler : IRequestHandler<DeleteDb1EntityCommand>
    {
        private readonly IDb1EntityRepository _db1EntityRepository;

        [IntentManaged(Mode.Merge)]
        public DeleteDb1EntityCommandHandler(IDb1EntityRepository db1EntityRepository)
        {
            _db1EntityRepository = db1EntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(DeleteDb1EntityCommand request, CancellationToken cancellationToken)
        {
            var db1Entity = await _db1EntityRepository.FindByIdAsync(request.Id, cancellationToken);
            if (db1Entity is null)
            {
                throw new NotFoundException($"Could not find Db1Entity '{request.Id}'");
            }

            _db1EntityRepository.Remove(db1Entity);
        }
    }
}