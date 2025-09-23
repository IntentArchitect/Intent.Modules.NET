using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Entities;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntityRepositoryInterface", Version = "1.0")]

namespace AwsLambdaFunction.Domain.Repositories
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public interface IDynClientRepository : IDynamoDBRepository<DynClient, string>
    {
    }
}