using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.ClassicDomainServiceTests;
using AdvancedMappingCrud.Repositories.Tests.Application.Interfaces;
using AdvancedMappingCrud.Repositories.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories.DomainServices;
using AdvancedMappingCrud.Repositories.Tests.Domain.Services.DomainServices;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ClassicDomainServiceTestsService : IClassicDomainServiceTestsService
    {
        private readonly IClassicDomainServiceTestRepository _classicDomainServiceTestRepository;
        private readonly IMyDomainService _myDomainService;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public ClassicDomainServiceTestsService(IClassicDomainServiceTestRepository classicDomainServiceTestRepository,
            IMyDomainService myDomainService,
            IMapper mapper)
        {
            _classicDomainServiceTestRepository = classicDomainServiceTestRepository;
            _myDomainService = myDomainService;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateClassicDomainServiceTest(
            ClassicDomainServiceTestCreateDto dto,
            CancellationToken cancellationToken = default)
        {
            var classicDomainServiceTest = new ClassicDomainServiceTest(
                service: _myDomainService);

            _classicDomainServiceTestRepository.Add(classicDomainServiceTest);
            await _classicDomainServiceTestRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return classicDomainServiceTest.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<ClassicDomainServiceTestDto> FindClassicDomainServiceTestById(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var classicDomainServiceTest = await _classicDomainServiceTestRepository.FindByIdAsync(id, cancellationToken);
            if (classicDomainServiceTest is null)
            {
                throw new NotFoundException($"Could not find ClassicDomainServiceTest '{id}'");
            }
            return classicDomainServiceTest.MapToClassicDomainServiceTestDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<ClassicDomainServiceTestDto>> FindClassicDomainServiceTests(CancellationToken cancellationToken = default)
        {
            var classicDomainServiceTests = await _classicDomainServiceTestRepository.FindAllAsync(cancellationToken);
            return classicDomainServiceTests.MapToClassicDomainServiceTestDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateClassicDomainServiceTest(
            Guid id,
            ClassicDomainServiceTestUpdateDto dto,
            CancellationToken cancellationToken = default)
        {
            var classicDomainServiceTest = await _classicDomainServiceTestRepository.FindByIdAsync(id, cancellationToken);
            if (classicDomainServiceTest is null)
            {
                throw new NotFoundException($"Could not find ClassicDomainServiceTest '{id}'");
            }

            classicDomainServiceTest.ClassicOp(_myDomainService);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteClassicDomainServiceTest(Guid id, CancellationToken cancellationToken = default)
        {
            var classicDomainServiceTest = await _classicDomainServiceTestRepository.FindByIdAsync(id, cancellationToken);
            if (classicDomainServiceTest is null)
            {
                throw new NotFoundException($"Could not find ClassicDomainServiceTest '{id}'");
            }

            _classicDomainServiceTestRepository.Remove(classicDomainServiceTest);
        }

        public void Dispose()
        {
        }
    }
}