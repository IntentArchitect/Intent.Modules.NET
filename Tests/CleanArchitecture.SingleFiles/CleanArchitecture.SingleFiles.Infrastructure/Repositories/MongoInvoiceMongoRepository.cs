using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.SingleFiles.Domain.Entities;
using CleanArchitecture.SingleFiles.Domain.Repositories;
using CleanArchitecture.SingleFiles.Domain.Repositories.Documents;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence;
using CleanArchitecture.SingleFiles.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace CleanArchitecture.SingleFiles.Infrastructure.Repositories
{
    internal class MongoInvoiceMongoRepository : MongoRepositoryBase<MongoInvoice, MongoInvoiceDocument, IMongoInvoiceDocument, string>, IMongoInvoiceRepository
    {
        public MongoInvoiceMongoRepository(IMongoCollection<MongoInvoiceDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}