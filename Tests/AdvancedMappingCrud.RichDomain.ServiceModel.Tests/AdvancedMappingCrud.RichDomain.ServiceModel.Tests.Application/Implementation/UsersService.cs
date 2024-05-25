using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Interfaces;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Users;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Contracts;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Entities;
using AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.RichDomain.ServiceModel.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class UsersService : IUsersService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public UsersService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var user = new User(
                companyId: dto.CompanyId,
                contactDetailsVO: new ContactDetailsVO(
                    cell: dto.ContactDetailsVO.Cell,
                    email: dto.ContactDetailsVO.Email),
                addresses: dto.Addresses
                    .Select(a => new AddressDC(
                        line1: a.Line1,
                        line2: a.Line2,
                        city: a.City,
                        postal: a.Postal))
                    .ToList());

            _userRepository.Add(user);
            await _userRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
            return user.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }
            return user.MapToUserDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.FindAllAsync(cancellationToken);
            return users.MapToUserDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteUser(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            _userRepository.Remove(user);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestCompanyResultDto> TestEntity(
            Guid id,
            TestEntityDto dto,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            var testEntityResult = user.TestEntity();
            return testEntityResult.MapToTestCompanyResultDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestAddressDCResultDto?> TestDC(
            Guid id,
            TestDCDto dto,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            var testDCResult = user.TestDC(dto.Index);
            return testDCResult?.MapToTestAddressDCResultDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<TestContactDetailsVOResultDto> TestVO(
            Guid id,
            TestVODto dto,
            CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            var testVOResult = user.TestVO();
            return testVOResult.MapToTestContactDetailsVOResultDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task AddCollections(Guid id, AddCollectionsDto dto, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.FindByIdAsync(id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            user.AddCollections(dto.Addresses
                .Select(a => new AddressDC(
                    line1: a.Line1,
                    line2: a.Line2,
                    city: a.City,
                    postal: a.Postal))
                .ToList(), dto.Contacts
                .Select(c => new ContactDetailsVO(
                    cell: c.Cell,
                    email: c.Email))
                .ToList());
        }

        public void Dispose()
        {
        }
    }
}