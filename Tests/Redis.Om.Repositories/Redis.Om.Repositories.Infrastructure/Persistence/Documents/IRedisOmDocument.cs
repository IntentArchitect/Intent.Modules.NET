using System;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.RedisOmDocumentOfTInterface", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Persistence.Documents
{
    internal interface IRedisOmDocument<TDomain, out TDocument>
        where TDomain : class
        where TDocument : IRedisOmDocument<TDomain, TDocument>
    {
        TDocument PopulateFromEntity(TDomain entity);
        TDomain ToEntity(TDomain? entity = null);
    }
}