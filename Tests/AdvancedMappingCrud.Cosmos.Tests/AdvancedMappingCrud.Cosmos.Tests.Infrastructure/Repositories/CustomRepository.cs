using System;
using AdvancedMappingCrud.Cosmos.Tests.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace AdvancedMappingCrud.Cosmos.Tests.Infrastructure.Repositories
{
    public class CustomRepository : ICustomRepository
    {
        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public int CustomRepoOperation()
        {
            // TODO: Implement CustomRepoOperation (CustomRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}