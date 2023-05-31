using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Entities.PrivateSetters.MongoDb.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.RepositoryInterface", Version = "1.0")]

namespace Entities.PrivateSetters.MongoDb.Domain.Repositories
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public interface IRepository<in TDomain>
    {
        void Add(TDomain entity);
        void Update(TDomain entity);
        void Remove(TDomain entity);
    }
}