using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using MultipleDocumentStores.Domain.Entities;
using MultipleDocumentStores.Domain.Repositories;
using MultipleDocumentStores.Domain.Repositories.Documents;
using MultipleDocumentStores.Infrastructure.Persistence;
using MultipleDocumentStores.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace MultipleDocumentStores.Infrastructure.Repositories
{
    internal class CustomerMongoMongoRepository : MongoRepositoryBase<CustomerMongo, CustomerMongoDocument, ICustomerMongoDocument, string>, ICustomerMongoRepository
    {
        public CustomerMongoMongoRepository(IMongoCollection<CustomerMongoDocument> collection,
            MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}