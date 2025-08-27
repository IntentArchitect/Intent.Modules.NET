using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.ToManyIds;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.ToManyIds;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.ToManyIds
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IToManySourceRepository : IMongoRepository<ToManySource, IToManySourceDocument, string>
    {
    }
}