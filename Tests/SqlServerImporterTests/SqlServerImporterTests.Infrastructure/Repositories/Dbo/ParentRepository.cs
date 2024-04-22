using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using SqlServerImporterTests.Domain.Entities.Dbo;
using SqlServerImporterTests.Domain.Repositories;
using SqlServerImporterTests.Domain.Repositories.Dbo;
using SqlServerImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Repositories.Dbo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ParentRepository : RepositoryBase<Parent, Parent, ApplicationDbContext>, IParentRepository
    {
        public ParentRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<Parent?> FindByIdAsync((Guid Id, Guid Id2) id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id.Id && x.Id2 == id.Id2, cancellationToken);
        }
    }
}