using System;
using System.Linq.Expressions;
using AdvancedMappingCrudMongo.Tests.Domain.Entities;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories;
using AdvancedMappingCrudMongo.Tests.Domain.Repositories.Documents;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence;
using AdvancedMappingCrudMongo.Tests.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AdvancedMappingCrudMongo.Tests.Infrastructure.Repositories
{
    internal class ProductMongoRepository : MongoRepositoryBase<Product, ProductDocument, IProductDocument, string>, IProductRepository
    {
        public ProductMongoRepository(IMongoCollection<ProductDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}