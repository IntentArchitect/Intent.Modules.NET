using System;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUserAddressById
{
    public class GetUserAddressByIdQuery : IRequest<UserAddressDto>, IQuery
    {
        public GetUserAddressByIdQuery(Guid userId, Guid id)
        {
            UserId = userId;
            Id = id;
        }

        public Guid UserId { get; set; }
        public Guid Id { get; set; }
    }
}