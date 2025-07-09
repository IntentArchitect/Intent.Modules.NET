using System;
using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ServiceToServiceInvocations;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ServiceToServiceInvocations;
using AdvancedMappingCrud.Repositories.Tests.Infrastructure.Persistence;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepository", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Infrastructure.Repositories.ServiceToServiceInvocations
{
    public class ServiceStoredProcRepository : IServiceStoredProcRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ServiceStoredProcRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public List<GetDataEntry> GetData()
        {
            // TODO: Implement GetData (ServiceStoredProcRepository) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}