using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using GraphQL.MongoDb.TestApplication.Domain.Repositories.Documents;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocument", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Persistence.Documents
{
    internal class UserDocument : IUserDocument, IMongoDbDocument<User, UserDocument, string>
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public IEnumerable<IAssignedPrivilegeDocument> AssignedPrivileges { get; set; }

        public User ToEntity(User? entity = default)
        {
            entity ??= ReflectionHelper.CreateNewInstanceOf<User>();

            entity.Id = Id ?? throw new Exception($"{nameof(entity.Id)} is null");
            entity.Name = Name ?? throw new Exception($"{nameof(entity.Name)} is null");
            entity.Surname = Surname ?? throw new Exception($"{nameof(entity.Surname)} is null");
            entity.Email = Email ?? throw new Exception($"{nameof(entity.Email)} is null");
            entity.AssignedPrivileges = AssignedPrivileges.Select(x => (x as AssignedPrivilegeDocument).ToEntity()).ToList();

            return entity;
        }

        public UserDocument PopulateFromEntity(User entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Surname = entity.Surname;
            Email = entity.Email;
            AssignedPrivileges = entity.AssignedPrivileges.Select(x => AssignedPrivilegeDocument.FromEntity(x)!).ToList();

            return this;
        }

        public static UserDocument? FromEntity(User? entity)
        {
            if (entity is null)
            {
                return null;
            }

            return new UserDocument().PopulateFromEntity(entity);
        }

        public static FilterDefinition<UserDocument> GetIdFilter(string id)
        {
            return Builders<UserDocument>.Filter.Eq(d => d.Id, id);
        }

        public FilterDefinition<UserDocument> GetIdFilter() => GetIdFilter(Id);

        public static FilterDefinition<UserDocument> GetIdsFilter(string[] ids)
        {
            return Builders<UserDocument>.Filter.In(d => d.Id, ids);
        }

        public static Expression<Func<UserDocument, bool>> GetIdFilterPredicate(string id)
        {
            return x => x.Id == id;
        }

        public static Expression<Func<UserDocument, bool>> GetIdsFilterPredicate(string[] ids)
        {
            return x => ids.Contains(x.Id);
        }
    }
}