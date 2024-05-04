using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Entities;
using EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace EntityFrameworkCore.MultiDbContext.NoDefaultDbContext.Application.Db1Entities.CreateDb1Entity
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CreateDb1EntityCommandHandler : IRequestHandler<CreateDb1EntityCommand, Guid>
    {
        private readonly IDb1EntityRepository _db1EntityRepository;

        [IntentManaged(Mode.Merge)]
        public CreateDb1EntityCommandHandler(IDb1EntityRepository db1EntityRepository)
        {
            _db1EntityRepository = db1EntityRepository;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> Handle(CreateDb1EntityCommand request, CancellationToken cancellationToken)
        {
            var db1Entity = new Db1Entity
            {
                Message = request.Message
            };

            _db1EntityRepository.Add(db1Entity);
            await _db1EntityRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return db1Entity.Id;
        }
    }
}