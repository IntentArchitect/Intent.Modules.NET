using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Domain.Contracts.ServiceToServiceInvocations;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.CustomRepositoryInterface", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ServiceToServiceInvocations
{
    public interface IServiceStoredProcRepository
    {
        List<GetDataEntry> GetData();
    }
}