using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using RichDomain.Domain.Entities;
using RichDomain.Domain.Repositories;
using RichDomain.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace RichDomain.Infrastructure.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class DerivedClassRepository : RepositoryBase<IDerivedClass, DerivedClass, ApplicationDbContext>, IDerivedClassRepository
    {
        public DerivedClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}