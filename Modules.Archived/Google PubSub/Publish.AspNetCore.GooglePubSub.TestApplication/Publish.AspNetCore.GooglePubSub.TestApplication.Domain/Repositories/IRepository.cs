using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Publish.AspNetCore.GooglePubSub.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.RepositoryInterface", Version = "1.0")]

namespace Publish.AspNetCore.GooglePubSub.TestApplication.Domain.Repositories
{
    [IntentManaged(Mode.Fully, Signature = Mode.Fully)]
    public interface IRepository<in TDomain>
    {
        void Add(TDomain entity);
        void Update(TDomain entity);
        void Remove(TDomain entity);
    }
}