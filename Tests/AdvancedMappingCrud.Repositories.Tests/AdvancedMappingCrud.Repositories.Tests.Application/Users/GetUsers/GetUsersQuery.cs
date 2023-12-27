using System.Collections.Generic;
using AdvancedMappingCrud.Repositories.Tests.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.QueryModels", Version = "1.0")]

namespace AdvancedMappingCrud.Repositories.Tests.Application.Users.GetUsers
{
    public class GetUsersQuery : IRequest<List<UserDto>>, IQuery
    {
        public GetUsersQuery(string? name, string? surname)
        {
            Name = name;
            Surname = surname;
        }

        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}