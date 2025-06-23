using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces.ServiceToServiceInvocation;
using AdvancedMappingCrud.Repositories.Tests.Application.ServiceToServiceInvocation;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.ServiceToServiceInvocations;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation.ServiceToServiceInvocation
{
    [IntentManaged(Mode.Merge)]
    public class StoredProcService : IStoredProcService
    {
        private readonly IServiceStoredProcRepository _serviceStoredProcRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public StoredProcService(IServiceStoredProcRepository serviceStoredProcRepository, IMapper mapper)
        {
            _serviceStoredProcRepository = serviceStoredProcRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<GetDataEntryDto>> GetData(CancellationToken cancellationToken = default)
        {
            var result = _serviceStoredProcRepository.GetData();
            return result.MapToGetDataEntryDtoList(_mapper);
        }
    }
}