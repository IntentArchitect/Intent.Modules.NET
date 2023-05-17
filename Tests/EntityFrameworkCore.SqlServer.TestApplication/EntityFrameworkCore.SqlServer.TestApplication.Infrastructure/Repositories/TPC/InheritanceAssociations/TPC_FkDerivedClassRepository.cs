using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Entities.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Domain.Repositories.TPC.InheritanceAssociations;
using EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace EntityFrameworkCore.SqlServer.TestApplication.Infrastructure.Repositories.TPC.InheritanceAssociations
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class TPC_FkDerivedClassRepository : RepositoryBase<TPC_FkDerivedClass, TPC_FkDerivedClass, ApplicationDbContext>, ITPC_FkDerivedClassRepository
    {
        public TPC_FkDerivedClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}