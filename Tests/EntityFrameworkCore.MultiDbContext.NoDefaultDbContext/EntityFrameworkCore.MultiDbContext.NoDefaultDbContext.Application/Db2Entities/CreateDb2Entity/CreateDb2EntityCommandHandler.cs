using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db2Entities.CreateDb2Entity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDb2EntityCommandHandler : IRequestHandler<CreateDb2EntityCommand, Guid>
    {
        private readonly IDb2EntityRepository _db2EntityRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDb2EntityCommandHandler(IDb2EntityRepository db2EntityRepository)
        {
            _db2EntityRepository = db2EntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateDb2EntityCommand request, CancellationToken cancellationToken)
        {
            var db2Entity = new Db2Entity
            {
                Message = request.Message
            };

            _db2EntityRepository.Add(db2Entity);
            await _db2EntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return db2Entity.Id;
        }
    }
}