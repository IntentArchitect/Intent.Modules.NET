using System;
using System.Linq.Expressions;
using Intent.RoslynWeaver.Attributes;
using MongoDB.Driver;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.MongoDb.MongoDbDocumentOfTInterface", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Infrastructure.Persistence.Documents
{
    internal interface IMongoDbDocument<TDomain, TDocument, TIdentifier>
        where TDomain : class
        where TDocument : IMongoDbDocument<TDomain, TDocument, TIdentifier>
    {
        TDocument PopulateFromEntity(TDomain entity);
        TDomain ToEntity(TDomain? entity = null);
        static abstract FilterDefinition<TDocument> GetIdFilter(TIdentifier id);
        FilterDefinition<TDocument> GetIdFilter();
        static abstract FilterDefinition<TDocument> GetIdsFilter(TIdentifier[] ids);
        static abstract Expression<Func<TDocument, bool>> GetIdFilterPredicate(TIdentifier id);
        static abstract Expression<Func<TDocument, bool>> GetIdsFilterPredicate(TIdentifier[] ids);
    }
}