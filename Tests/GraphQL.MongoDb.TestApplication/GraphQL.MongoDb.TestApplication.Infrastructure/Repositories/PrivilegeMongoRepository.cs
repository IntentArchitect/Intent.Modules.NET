using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using GraphQL.MongoDb.TestApplication.Domain.Repositories;
using GraphQL.MongoDb.TestApplication.Domain.Repositories.Documents;
using GraphQL.MongoDb.TestApplication.Infrastructure.Persistence;
using GraphQL.MongoDb.TestApplication.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Repositories
{
    internal class PrivilegeMongoRepository : MongoRepositoryBase<Privilege, PrivilegeDocument, IPrivilegeDocument, string>, IPrivilegeRepository
    {
        public PrivilegeMongoRepository(IMongoCollection<PrivilegeDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}