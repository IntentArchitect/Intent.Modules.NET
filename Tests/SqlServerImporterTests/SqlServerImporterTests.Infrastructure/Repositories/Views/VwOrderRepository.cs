using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using SqlServerImporterTests.Domain.Entities.Views;
using SqlServerImporterTests.Domain.Repositories;
using SqlServerImporterTests.Domain.Repositories.Views;
using SqlServerImporterTests.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Repository", Version = "1.0")]

namespace SqlServerImporterTests.Infrastructure.Repositories.Views
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class VwOrderRepository : RepositoryBase<VwOrder, VwOrder, ApplicationDbContext>, IVwOrderRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public VwOrderRepository(ApplicationDbContext dbContext, IMapper mapper) : base(dbContext, mapper)
        {
            _dbContext = dbContext;
        }

        public void Add(VwOrder entity)
        {
            _dbContext.Database.ExecuteSqlInterpolated($"INSERT INTO VwOrders (Id, CustomerId, OrderDate, RefNo) VALUES({entity.Id}, {entity.CustomerId}, {entity.OrderDate}, {entity.RefNo})");
        }
    }
}