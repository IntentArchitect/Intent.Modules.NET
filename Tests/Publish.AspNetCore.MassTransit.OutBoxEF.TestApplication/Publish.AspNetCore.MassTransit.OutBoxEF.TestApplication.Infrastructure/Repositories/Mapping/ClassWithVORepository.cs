using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities.Mapping;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Repositories;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Repositories.Mapping;
using Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Infrastructure.Repositories.Mapping
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class ClassWithVORepository : RepositoryBase<ClassWithVO, ClassWithVO, ApplicationDbContext>, IClassWithVORepository
    {
        public ClassWithVORepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
        }

        public async Task<ClassWithVO?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await FindAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<ClassWithVO>> FindByIdsAsync(Guid[] ids, CancellationToken cancellationToken = default)
        {
            return await FindAllAsync(x => ids.Contains(x.Id), cancellationToken);
        }
    }
}