using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AzureFunctions.MongoDb.Domain.Entities.Collections;
using AzureFunctions.MongoDb.Domain.Repositories.Documents.Collections;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AzureFunctions.MongoDb.Domain.Repositories.Collections
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface ICustomCollectionEntityARepository : IMongoRepository<CustomCollectionEntityA, ICustomCollectionEntityADocument, string>
    {
    }
}