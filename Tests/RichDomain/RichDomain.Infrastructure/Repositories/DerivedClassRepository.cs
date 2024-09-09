using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
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
        private readonly ApplicationDbContext _dbContext;
        public DerivedClassRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public void Add(IDerivedClass entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO DerivedClasses (DerivedAttribute) VALUES({entity.DerivedAttribute})");
        }
    }
}