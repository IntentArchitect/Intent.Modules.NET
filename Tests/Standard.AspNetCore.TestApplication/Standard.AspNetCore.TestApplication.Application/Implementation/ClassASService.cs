using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Standard.AspNetCore.TestApplication.Application.ClassAS;
using Standard.AspNetCore.TestApplication.Application.Interfaces;
using Standard.AspNetCore.TestApplication.Domain.Entities;
using Standard.AspNetCore.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace Standard.AspNetCore.TestApplication.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class ClassASService : IClassASService
    {
        private readonly IClassARepository _classARepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public ClassASService(IClassARepository classARepository, IMapper mapper)
        {
            _classARepository = classARepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Create(ClassACreateDTO dto)
        {
            var newClassA = new ClassA
            {
                Attribute = dto.Attribute,
            };

            _classARepository.Add(newClassA);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<ClassADTO> FindById(Guid id)
        {
            var element = await _classARepository.FindByIdAsync(id);
            return element.MapToClassADTO(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<List<ClassADTO>> FindAll()
        {
            var elements = await _classARepository.FindAllAsync();
            return elements.MapToClassADTOList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Update(Guid id, ClassAUpdateDTO dto)
        {
            var existingClassA = await _classARepository.FindByIdAsync(id);
            existingClassA.Id = dto.Id;
            existingClassA.Attribute = dto.Attribute;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Delete(Guid id)
        {
            var existingClassA = await _classARepository.FindByIdAsync(id);
            _classARepository.Remove(existingClassA);
        }

        public void Dispose()
        {
        }
    }
}