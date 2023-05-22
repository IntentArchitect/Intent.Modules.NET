using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using GraphQL.MongoDb.TestApplication.Application.Common.Mappings;
using GraphQL.MongoDb.TestApplication.Application.Privileges;
using GraphQL.MongoDb.TestApplication.Application.Privileges.GetPrivilegeById;
using GraphQL.MongoDb.TestApplication.Domain.Entities;
using HotChocolate;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace GraphQL.MongoDb.TestApplication.Application.Users
{
    public class UserAssignedPrivilegeDto : IMapFrom<AssignedPrivilege>
    {
        public UserAssignedPrivilegeDto()
        {
            PrivilegeId = null!;
        }

        public string PrivilegeId { get; set; }

        public static UserAssignedPrivilegeDto Create(string privilegeId)
        {
            return new UserAssignedPrivilegeDto
            {
                PrivilegeId = privilegeId
            };
        }

        public async Task<PrivilegeDto> GetPrivilege(CancellationToken cancellationToken, [Service] ISender mediator)
        {
            return await mediator.Send(new GetPrivilegeByIdQuery(id: PrivilegeId), cancellationToken);
        }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<AssignedPrivilege, UserAssignedPrivilegeDto>();
        }
    }
}