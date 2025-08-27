using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Domain.Entities;
using Entities.PrivateSetters.MongoDb.Domain.Repositories;
using Entities.PrivateSetters.MongoDb.Domain.Repositories.Documents;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence;
using Entities.PrivateSetters.MongoDb.Infrastructure.Persistence.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;
using Tag = Entities.PrivateSetters.MongoDb.Domain.Entities.Tag;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbRepository", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Infrastructure.Repositories
{
    internal class TagMongoRepository : MongoRepositoryBase<Tag, TagDocument, ITagDocument, string>, ITagRepository
    {
        public TagMongoRepository(IMongoCollection<TagDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}