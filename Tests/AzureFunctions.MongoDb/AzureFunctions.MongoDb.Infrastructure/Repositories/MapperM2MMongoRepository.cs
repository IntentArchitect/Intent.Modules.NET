using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities;
using AzureFunctions.MongoDb.Domain.Repositories;
using AzureFunctions.MongoDb.Domain.Repositories.Documents;
using AzureFunctions.MongoDb.Infrastructure.Persistence;
using AzureFunctions.MongoDb.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace AzureFunctions.MongoDb.Infrastructure.Repositories
{
    internal class MapperM2MMongoRepository : MongoRepositoryBase<MapperM2M, MapperM2MDocument, IMapperM2MDocument, string>, IMapperM2MRepository
    {
        public MapperM2MMongoRepository(IMongoCollection<MapperM2MDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}