using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDb.MultiTenancy.SeperateDb.Domain.Entities;
using MongoDb.MultiTenancy.SeperateDb.Domain.Repositories.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomerRepository : IMongoRepository<Customer, ICustomerDocument, string>
    {
    }
}