using System;
using System.Linq.Expressions;
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
    internal class UserMongoRepository : MongoRepositoryBase<User, UserDocument, IUserDocument, string>, IUserRepository
    {
        public UserMongoRepository(IMongoCollection<UserDocument> collection, MongoDbUnitOfWork unitOfWork) : base(collection, unitOfWork)
        {
        }
    }
}