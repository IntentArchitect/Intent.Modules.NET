using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.IdTypes;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.IdTypes;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.IdTypes
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IIdTypeGuidRepository : IMongoRepository<IdTypeGuid, IIdTypeGuidDocument, Guid>
    {
    }
}