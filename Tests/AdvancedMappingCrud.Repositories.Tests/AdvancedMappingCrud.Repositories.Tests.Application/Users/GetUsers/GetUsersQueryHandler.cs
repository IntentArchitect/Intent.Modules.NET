using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Pagination;
using AdvancedMappingCrud.Repositories.Tests.Domain.Entities;
using AdvancedMappingCrud.Repositories.Tests.Domain.Repositories;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryHandler", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUsers
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        [IntentManaged(Mode.Merge)]
        public GetUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            IQueryable<User> FilterUsers(IQueryable<User> queryable)
            {
                if (request.Name != null)
                {
                    queryable = queryable.Where(x => x.Name == request.Name);
                }

                if (request.Surname != null)
                {
                    queryable = queryable.Where(x => x.Surname == request.Surname);
                }

                return queryable;
            }

            var users = await _userRepository.FindAllAsync(request.PageNo, request.PageSize, FilterUsers, cancellationToken);
            return users.MapToPagedResult(x => x.MapToUserDto(_mapper));
        }
    }
}