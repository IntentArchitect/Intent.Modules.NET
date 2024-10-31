using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.DbContext.Tests.Application.Common.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Application.Interfaces;
using AdvancedMappingCrud.DbContext.Tests.Application.Users;
using AdvancedMappingCrud.DbContext.Tests.Domain.Common;
using AdvancedMappingCrud.DbContext.Tests.Domain.Common.Exceptions;
using AdvancedMappingCrud.DbContext.Tests.Domain.Entities;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations.ServiceImplementation", Version = "1.0")]

namespace AdvancedMappingCrud.DbContext.Tests.Application.Implementation
{
    [IntentManaged(Mode.Merge)]
    public class UsersService : IUsersService
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public UsersService(IApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<Guid> CreateUser(UserCreateDto dto, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return user.Id;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<UserDto> FindUserById(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }
            return user.MapToUserDto(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<List<UserDto>> FindUsers(CancellationToken cancellationToken = default)
        {
            var users = await _dbContext.Users
                .ToListAsync(cancellationToken);
            return users.MapToUserDtoList(_mapper);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task UpdateUser(
            Guid id,
            UserUpdateDto dto,
            List<NewDTO> addresses,
            CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.Addresses = UpdateHelper.CreateOrUpdateCollection(user.Addresses, addresses, (e, d) => e.Id == d.Id, CreateOrUpdateAddress);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task DeleteUser(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException($"Could not find User '{id}'");
            }

            _dbContext.Users.Remove(user);
        }

        [IntentManaged(Mode.Fully)]
        private static Address CreateOrUpdateAddress(Address? entity, NewDTO dto)
        {
            entity ??= new Address();
            entity.Id = dto.Id;
            entity.Line1 = dto.Line1;
            entity.Line2 = dto.Line2;
            entity.City = dto.City;
            entity.Postal = dto.Postal;
            return entity;
        }
    }
}