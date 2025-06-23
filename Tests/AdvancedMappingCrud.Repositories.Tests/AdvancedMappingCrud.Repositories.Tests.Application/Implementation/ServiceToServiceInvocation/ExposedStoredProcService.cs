using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.ServiceToServiceInvocation;
using AdvancedMappingCrud.Repositories.Tests.Application.ServiceToServiceInvocation;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation.ServiceToServiceInvocation
{
    [IntentManaged(Mode.Merge)]
    public class ExposedStoredProcService : IExposedStoredProcService
    {
        private readonly IStoredProcService _storedProcService;

        [IntentManaged(Mode.Merge)]
        public ExposedStoredProcService(IStoredProcService storedProcService)
        {
            _storedProcService = storedProcService;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<GetDataEntryDto>> GetData(CancellationToken cancellationToken = default)
        {
            var result = await _storedProcService.GetData(cancellationToken);
            return result;
        }
    }
}